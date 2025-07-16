using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace NDScript.Syntax
{
    public static class Parser
    {
        /// <summary>
        /// Things that aren't valid identifier names
        /// </summary>
        public static readonly string[] Keywords = { "fail", "choose", "var", "function", "or", "else", "foreach", "in", "return", "true", "false", "null" };

        public static readonly Parser<Expression> Expression = Parse.Ref(() => RealExpression);
        public static readonly Parser<Statement> Statement = Parse.Ref(() => RealStatement);

        public static readonly Parser<Statement[]> StatementSequence =
            from s in Statement.AtLeastOnce()
            select s.ToArray();

        public static readonly CommentParser CommentStyle = new CommentParser();

        #region Utilities

        private static Parser<T> Commit<T>(this Parser<T> parser) =>
            input =>
            {
                var result = parser(input);
                if (result.WasSuccessful)
                    return result;
                throw new ParseException(result.ToString(), Sprache.Position.FromInput(result.Remainder));
            };

        public static Parser<T> ChainOperatorCommitted<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainOperatorRestCommitted(first, op, operand, apply, Parse.Or));
        }

        static Parser<T> ChainOperatorRestCommitted<T, TOp>(
            T firstOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return or(op.Then(opValue =>
                    operand.Commit().Then(operandValue =>
                        ChainOperatorRestCommitted(apply(opValue, firstOperand, operandValue), op, operand, apply, or))),
                Parse.Return(firstOperand));
        }

        public static Parser<IEnumerable<T>> DelimitedByCommited<T, U>(this Parser<T> parser, Parser<U> delimiter, int? minimumCount, int? maximumCount)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return from head in parser.Once()
                from tail in
                    (from separator in delimiter
                        from item in parser.Commit()
                        select item).Repeat(minimumCount - 1, maximumCount - 1)
                select head.Concat(tail);
        }

        private static Parser<T> Bracketed<T>(char open, Parser<T> enclosed, char close) =>
            from start in Parse.Char(open).Token()
            from payload in enclosed.Commit()
            from end in Parse.Char(close).Token().Commit()
            select payload;

        private static Parser<T> Parenthesized<T>(this Parser<T> enclosed) => Bracketed('(', enclosed, ')');

        public static Parser<IEnumerable<T>> CommaSeparatedList<T>(this Parser<T> element) =>
            (element).DelimitedByCommited(
                Parse.Char(',').Token(), null, null).Or(Parse.Return(Array.Empty<T>())).Token();

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
            (from id in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Token()
                where !Keywords.Contains(id)
                select id).Named("identifier");

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
            from indices in Bracketed('[', Expression.Commit().CommaSeparatedList(), ']')
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
            ChainOperatorCommitted(Times.Or(Divide), UnaryExpression.Named("expression"), 
                (op, left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region AdditiveExpression
        private static readonly Parser<Func<object?, object?, object?>> Plus =
            Parse.Char('+').Return(BinaryOperatorExpression.Add).Token();
        private static readonly Parser<Func<object?, object?, object?>> Minus =
            Parse.Char('-').Return(BinaryOperatorExpression.Subtract).Token();

        public static readonly Parser<Expression> AdditiveExpression =
            ChainOperatorCommitted(Plus.Or(Minus), MultiplicativeExpression.Named("expression"),
                (op, left, right) => new BinaryOperatorExpression(op, left, right));
        #endregion

        #region Relational expression
        private static readonly Parser<Func<object?, object?, object?>> RelationalOperator =
            (Parse.String("<=").Return(BinaryOperatorExpression.Le)).Or((Parse.String(">=").Return(BinaryOperatorExpression.Ge)))
            .Or((Parse.Char('<').Return(BinaryOperatorExpression.Lt)))
            .Or((Parse.Char('>').Return(BinaryOperatorExpression.Lt))).Token().Named("relational operator");

        private static Parser<TExp> RelationExpressionCompletion<TExp, TRel>(TExp left, Parser<TRel> op, Parser<TExp> exp, Func<TExp, TRel, TExp, TExp> result) =>
            (from o in op
                from right in exp.Commit()
                select result(left, o, right));

        public static readonly Parser<Expression> RelationalExpression =
            AdditiveExpression.Then(left => RelationExpressionCompletion(left, RelationalOperator, AdditiveExpression.Named("expression"),
                    (l, op, r) => new BinaryOperatorExpression(op, l, r))
                .Or(Parse.Return(left)));
        #endregion

        #region Equality expression
        private static readonly Parser<Func<object?, object?, object?>> EqualityOperator =
            (Parse.String("==").Return(BinaryOperatorExpression.Eq)
                .Or((Parse.String("!=").Return(BinaryOperatorExpression.Neq)))).Token().Named("relational operator");

        public static readonly Parser<Expression> EqualityExpression =
            RelationalExpression.Then(left => RelationExpressionCompletion(left, EqualityOperator, RelationalExpression.Named("expression"),
                    (l, op, r) => new BinaryOperatorExpression(op, l, r))
                .Or(Parse.Return(left)));
        #endregion

        #region Logical expressions
        private static readonly Parser<Expression> LogicalAndExpression =
            ChainOperatorCommitted(Parse.String("&&").Token(), EqualityExpression.Named("expression"),
                (_, left, right) => new ShortCircuitOperatorExpression(false, left, right));

        private static readonly Parser<Expression> LogicalOrExpression =
            ChainOperatorCommitted(Parse.String("||").Token(), LogicalAndExpression.Named("expression"),
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
                from name in Identifier.Commit()
                from _ in Parse.Char('=').Token().Commit()
                from value in Expression.Named("expression").Commit()
                select new VariableDeclaration(name, value)).Token().Named("variable declaration");

        private static readonly Parser<Statement> ReturnStatement =
            (from _ in Parse.String("return").Token()
                from value in Expression.Commit()
                select new Return(value)).Token().Named("return statement");

        private static readonly Parser<Statement> SimpleStatement =
            from s in VariableDeclaration.Or(ReturnStatement).Or(Expression)
            from _ in Parse.Char(';').Token()
            select s;
        #endregion

        #region Compound statements
        public static readonly Parser<Block> Block =
            (from statements in Bracketed('{', Statement.Many(), '}')
            select new Block(statements.ToArray())).Named("block");

        private static Parser<bool> DeterminismTag = (Parse.String("deterministic").Return(true)).Or(Parse.Return(false));

        public static readonly Parser<FunctionDeclaration> FunctionDefinition =
            (from det in DeterminismTag
                from _ in Parse.String("function").Token()
            from name in Identifier.Commit()
            from args in Parenthesized(CommaSeparatedList(Identifier)).Commit()
            from b in Block.Commit()
            select new FunctionDeclaration(name, det, args.ToArray(), b)).Named("function definition");

        private static readonly Parser<Statement?> OptionalElse =
            (from _ in Parse.String("else").Token()
                from alt in Statement
                select alt).Or(Parse.Return<Statement?>(null));

        public static readonly Parser<Statement> IfStatement =
            (from _ in Parse.String("if").Token()
            from _2 in Parse.Char('(').Token().Commit()
            from condition in Expression.Commit()
            from _3 in Parse.Char(')').Token().Commit()
            from consequent in Statement.Commit()
            from alternative in OptionalElse
            select new IfStatement(condition, consequent, alternative)).Named("if statement");

        public static readonly Parser<Statement> WhileStatement =
            (from _ in Parse.String("while").Token()
                from _2 in Parse.Char('(').Token().Commit()
                from condition in Expression.Commit()
                from _3 in Parse.Char(')').Token().Commit()
                from body in Statement.Commit()
                select new WhileStatement(condition, body)).Named("while statement");

        private static readonly Parser<Statement> OptionClause =
            (from _ in Parse.String("or").Token()
            from s in Statement.Commit()
            select s).Named("option");

        public static readonly Parser<Statement> ChooseStatement =
            (from _ in Parse.String("choose").Token()
                from mode in (Parse.String("first").Token().Return(true)).Or(Parse.Return(false))
                from first in Statement.Commit()
                from rest in OptionClause.Many()
                select new ChooseStatement(rest.Prepend(first).ToArray(), mode)).Named("choose statement");

        public static readonly Parser<Statement> ForeachStatement =
            (from _ in Parse.String("foreach").Token()
                from open in Parse.Char('(').Token().Commit()
                from name in Identifier.Named("variable").Commit()
                from _2 in Parse.String("in").Token().Commit()
                from collection in Expression.Named("collection expression").Commit()
                from close in Parse.Char(')').Token().Commit()
                from body in Statement.Named("body").Commit()
                select new ForeachStatement(name, collection, body)).Named("foreach statement");
        #endregion

        private static readonly Parser<Statement> RealStatement =
            SimpleStatement.Or(Block).Or(FunctionDefinition).Or(IfStatement).Or(WhileStatement).Or(ChooseStatement).Or(ForeachStatement).Named("statement");
    }
}
