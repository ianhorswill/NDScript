using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace NDScript.Syntax
{
    public static class Parser
    {
        public static readonly Parser<Expression> Expression = Parse.Ref(() => RealExpression);
        public static readonly Parser<Statement> Statement = Parse.Ref(() => RealStatement);

        public static readonly Parser<Statement[]> StatementSequence =
            from s in Statement.AtLeastOnce()
            select s.ToArray();

        public static readonly CommentParser CommentStyle = new CommentParser();

        #region Utilities
        private static Parser<T> Bracketed<T>(char open, Parser<T> enclosed, char close) =>
            enclosed.Contained(Parse.Char(open).Token(), Parse.Char(close).Token());

        private static Parser<T> Parenthesized<T>(this Parser<T> enclosed) => Bracketed('(', enclosed, ')');

        public static Parser<IEnumerable<T>> CommaSeparatedList<T>(this Parser<T> element) =>
            (element).DelimitedBy(Parse.Char(',').Token()).Or(Parse.Return(Array.Empty<T>())).Token();

        public static readonly Parser<IEnumerable<Expression>> Arglist =
            Expression.CommaSeparatedList().Parenthesized();
        #endregion

        #region Primary expressions
        public static readonly Parser<Expression> StringLiteral =
            (from open in Parse.Char('"')
                from str in Parse.CharExcept('"').Many().Text()
                from close in Parse.Char('"')
                select new Constant(str)).Named("string").Token();

        public static readonly Parser<Expression> ArrayExpression =
            (from elements in Bracketed('[', Expression.CommaSeparatedList(), ']')
            select new ArrayExpression(elements.ToArray())).Named("array expression");

        private static readonly Parser<string> Identifier =
            Parse.Identifier(Parse.Letter, Parse.LetterOrDigit)
                .Except(Parse.String("fail").Or(Parse.String("choose")).Or(Parse.String("var")))
                    .Token().Named("identifier");

        private static readonly Parser<int> Sign =
            Parse.Char('+').Return(1).XOr(Parse.Char('-').Return(-1)).Or(Parse.Return(1)).Named("sign");

        public static readonly Parser<Expression> Number =
            (from sign in Sign
             from text in Parse.DecimalInvariant
             select int.TryParse(text, out var n)? new Constant(sign * n) : new Constant(sign*float.Parse(text))
            ).Named("number");

        private static readonly Parser<Expression> True = Parse.String("true").Return(new Constant(true));
        private static readonly Parser<Expression> False = Parse.String("false").Return(new Constant(false));
        private static readonly Parser<Expression> Null = Parse.String("null").Return(new Constant(null)).Named("null");

        public static readonly Parser<Expression> Boolean = True.Or(False).Named("Boolean");

        public static readonly Parser<Expression> Literal = (Number.XOr(StringLiteral).Or(Boolean).Or(Null)).Token();

        public static readonly Parser<Expression> VariableReference =
            (from name in Identifier
            select new VariableReference(name)).Named("variable reference");
        
        public static readonly Parser<Expression> FailExpression = Parse.String("fail").Token().Return(Fail.Singleton);

        public static readonly Parser<Expression> ChooseExpression =
            (from _ in Parse.String("choose").Token()
                from alternatives in Arglist
                select new ChooseExpression(alternatives.ToArray())).Named("choose expression");

        public static readonly Parser<Expression> PrimitiveExpression = Literal.Or(ArrayExpression).Or(ChooseExpression).Or(VariableReference).Or(FailExpression).Or(Parenthesized(Expression));
        #endregion

        #region Postfix expression

        private static readonly Parser<Func<Expression, Expression>> FunctionCall =
            from arglist in Arglist
            select (Func<Expression,Expression>)(e => new FunctionCall(e, arglist.ToArray()));

        private static readonly Parser<Func<Expression, Expression>> ArrayReference =
            from indices in Bracketed('[', Expression.CommaSeparatedList(), ']')
            select (Func<Expression,Expression>)(e =>
            {
                var i = indices.ToArray();
                return i.Length switch
                {
                    1 => new ArrayReference(e, i[0]),
                    2 => new GridReference(e, i[0], i[1]),
                    _ => throw new ArgumentException($"Arrays with {i.Length} dimensions are not supported")
                };
            });

        private static readonly Parser<Func<Expression, Expression>> PostfixModifier = FunctionCall.Or(ArrayReference);

        public static readonly Parser<Expression> PosfixExpression =
            from e in PrimitiveExpression
            from modifiers in PostfixModifier.Many()
            select ApplyModifiers(e, modifiers);

        private static Expression ApplyModifiers(Expression expression, IEnumerable<Func<Expression, Expression>> modifiers) =>
            modifiers.Aggregate(expression, (current, m) => m(current));

        #endregion

        #region Unary expression
        public static readonly Parser<Expression> Negation =
            from _ in Parse.Char('-').Token()
            from v in PosfixExpression
            select new UnaryOperatorExpression(UnaryOperatorExpression.Negate, v);

        public static readonly Parser<Expression> NotExpression =
            from _ in Parse.Char('!').Token()
            from v in PosfixExpression
            select new UnaryOperatorExpression(UnaryOperatorExpression.Not, v);

        private static readonly Parser<Expression> UnaryExpression = Negation.Or(NotExpression).Or(PosfixExpression);
        #endregion

        #region Multiplicative expression
        private static readonly Parser<Func<object?, object?, object?>> Times =
            Parse.Char('*').Return(BinaryOperatorExpression.Multiply).Token();
        private static readonly Parser<Func<object?, object?, object?>> Divide =
            Parse.Char('/').Return(BinaryOperatorExpression.Divide).Token();

        public static readonly Parser<Expression> MultiplicativeExpression =
            Parse.ChainOperator(Times.Or(Divide), UnaryExpression, 
                (op, left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region AdditiveExpression
        private static readonly Parser<Func<object?, object?, object?>> Plus =
            Parse.Char('+').Return(BinaryOperatorExpression.Add).Token();
        private static readonly Parser<Func<object?, object?, object?>> Minus =
            Parse.Char('-').Return(BinaryOperatorExpression.Subtract).Token();

        public static readonly Parser<Expression> AdditiveExpression =
            Parse.ChainOperator(Plus.Or(Minus), MultiplicativeExpression,
                (op, left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region Relational expression
        private static readonly Parser<Func<object?, object?, object?>> RelationalOperator =
            (Parse.String("<=").Return(BinaryOperatorExpression.Le)).Or((Parse.String(">=").Return(BinaryOperatorExpression.Ge)))
            .Or((Parse.Char('<').Return(BinaryOperatorExpression.Le)))
            .Or((Parse.Char('>').Return(BinaryOperatorExpression.Le))).Token().Named("relational operator");

        public static readonly Parser<Expression> RelationalExpression =
            Parse.ChainOperator(RelationalOperator, AdditiveExpression, (op,
                left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region Equality expression
        private static readonly Parser<Func<object?, object?, object?>> EqualityOperator =
            (Parse.String("==").Return(BinaryOperatorExpression.Eq)
                .Or((Parse.String("!=").Return(BinaryOperatorExpression.Neq)))).Token().Named("relational operator");

        public static readonly Parser<Expression> EqualityExpression =
            Parse.ChainOperator(EqualityOperator, RelationalExpression, (op,
                left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region Logical expressions
        private static readonly Parser<Expression> LogicalAndExpression =
            Parse.ChainOperator(Parse.String("&&").Token(), EqualityExpression,
                (_, left, right) => new ShortCircuitOperatorExpression(false, left, right));

        private static readonly Parser<Expression> LogicalOrExpression =
            Parse.ChainOperator(Parse.String("||").Token(), LogicalAndExpression,
                (_, left, right) => new ShortCircuitOperatorExpression(true, left, right));
        #endregion

        public static readonly Parser<Expression> AssignmentExpression =
            (from lvalue in PosfixExpression 
                from _ in Parse.Char('=').Token()
                from value in Expression
                select new AssignmentExpression(SettableExpression.CheckSettable(lvalue), value)).Token().Named("assignment expression");

        private static readonly Parser<Expression> RealExpression =
            AssignmentExpression.Or(LogicalOrExpression);
        
        #region Simple statements
        private static readonly Parser<Statement> VariableDeclaration =
            (from v in Parse.String("var").Token()
                from name in Identifier
                from _ in Parse.Char('=').Token()
                from value in Expression
                select new VariableDeclaration(name, value)).Token().Named("variable declaration");

        private static readonly Parser<Statement> ReturnStatement =
            (from _ in Parse.String("return").Token()
                from value in Expression
                select new Return(value)).Token().Named("return statement");

        private static readonly Parser<Statement> SimpleStatement =
            from s in VariableDeclaration.Or(ReturnStatement).Or(Expression)
            from _ in Parse.Char(';').Token()
            select s;
        #endregion

        #region Compound statements
        public static readonly Parser<Block> Block =
            (from statements in Bracketed('{', StatementSequence, '}')
            select new Block(statements)).Named("block");

        public static readonly Parser<FunctionDeclaration> FunctionDefinition =
            (from _ in Parse.String("function").Token()
            from name in Identifier
            from args in Parenthesized(CommaSeparatedList(Identifier))
            from b in Block
            select new FunctionDeclaration(name, args.ToArray(), b)).Named("function definition");

        private static readonly Parser<Statement?> OptionalElse =
            (from _ in Parse.String("else").Token()
                from alt in Statement
                select alt).Or(Parse.Return<Statement?>(null));

        public static readonly Parser<Statement> IfStatement =
            (from _ in Parse.String("if").Token()
            from _2 in Parse.Char('(').Token()
            from condition in Expression
            from _3 in Parse.Char(')').Token()
            from consequent in Statement
            from alternative in OptionalElse
            select new IfStatement(condition, consequent, alternative)).Named("if statement");

        public static readonly Parser<Statement> WhileStatement =
            (from _ in Parse.String("while").Token()
                from _2 in Parse.Char('(').Token()
                from condition in Expression
                from _3 in Parse.Char(')').Token()
                from body in Statement
                select new WhileStatement(condition, body)).Named("while statement");

        private static readonly Parser<Statement> OptionClause =
            (from _ in Parse.String("or").Token()
            from s in Statement
            select s).Named("option");

        public static readonly Parser<Statement> ChooseStatement =
            (from _ in Parse.String("choose").Token()
                from mode in (Parse.String("first").Token().Return(true)).Or(Parse.Return(false))
                from first in Statement
                from rest in OptionClause.Many()
                select new ChooseStatement(rest.Prepend(first).ToArray(), mode)).Named("choose statement");

        public static readonly Parser<Statement> ForeachStatement =
            (from _ in Parse.String("foreach").Token()
                from open in Parse.Char('(').Token()
                from name in Identifier
                from _2 in Parse.String("in").Token()
                from collection in Expression
                from close in Parse.Char(')').Token()
                from body in Statement
                select new ForeachStatement(name, collection, body)).Named("foreach statement");
        #endregion

        private static readonly Parser<Statement> RealStatement =
            SimpleStatement.Or(Block).Or(FunctionDefinition).Or(IfStatement).Or(WhileStatement).Or(ChooseStatement).Or(ForeachStatement).Named("statement");
    }
}
