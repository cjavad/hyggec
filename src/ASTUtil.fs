// hyggec - The didactic compiler for the Hygge programming language.
// Copyright (C) 2023 Technical University of Denmark
// Author: Alceste Scalas <alcsc@dtu.dk>
// Released under the MIT license (see LICENSE.md for details)

/// Utility functions to inspect and manipulate the Abstract Syntax Tree of
/// Hygge programs.
module ASTUtil

open AST


/// Given the AST 'node', return a new AST node where every free occurrence of
/// the variable called 'var' is substituted by the AST node 'sub'.
let rec subst (node: Node<'E,'T>) (var: string) (sub: Node<'E,'T>): Node<'E,'T> =
    match node.Expr with
    | UnitVal
    | IntVal(_)
    | BoolVal(_)
    | FloatVal(_)
    | StringVal(_) -> node // The substitution has no effect

    | Pointer(_) -> node // The substitution has no effect

    | Var(vname) when vname = var -> sub // Substitution applied
    | Var(_) -> node // The substitution has no effect
    
    | Sub(lhs, rhs) ->
        {node with Expr = Sub((subst lhs var sub), (subst rhs var sub))}
    | Add(lhs, rhs) ->
        {node with Expr = Add((subst lhs var sub), (subst rhs var sub))}
    | Div(lhs, rhs) ->
        {node with Expr = Div((subst lhs var sub), (subst rhs var sub))}
    | Mult(lhs, rhs) ->
        {node with Expr = Mult((subst lhs var sub), (subst rhs var sub))}
    | BNot(arg) -> 
        {node with Expr = BNot((subst arg var sub))}
    | BAnd(lhs, rhs) ->
        {node with Expr = BAnd((subst lhs var sub), (subst rhs var sub))}
    | BOr(lhs, rhs) ->
        {node with Expr = BOr((subst lhs var sub), (subst rhs var sub))}
    | BXor(lhs, rhs) ->
        {node with Expr = BXor((subst lhs var sub), (subst rhs var sub))}
    | BSL(lhs, rhs) ->
        {node with Expr = BSL((subst lhs var sub), (subst rhs var sub))}
    | BSR(lhs, rhs) ->
        {node with Expr = BSR((subst lhs var sub), (subst rhs var sub))}
    | Rem(lhs, rhs) ->
        {node with Expr = Rem((subst lhs var sub), (subst rhs var sub))}
    | Sqrt(arg) ->
        {node with Expr = Sqrt(subst arg var sub)}
    | And(lhs, rhs) ->
        {node with Expr = And((subst lhs var sub), (subst rhs var sub))}
    | ScAnd(lhs, rhs) ->
        {node with Expr = And((subst lhs var sub), (subst rhs var sub))}
    | Or(lhs, rhs) ->
        {node with Expr = Or((subst lhs var sub), (subst rhs var sub))}
    | ScOr(lhs, rhs) ->
        {node with Expr = Or((subst lhs var sub), (subst rhs var sub))}
    | Xor(lhs, rhs) ->
        {node with Expr = Xor((subst lhs var sub), (subst rhs var sub))}
    | Not(arg) ->
        {node with Expr = Not(subst arg var sub)}
    | Neg(arg) ->
        {node with Expr = Neg(subst arg var sub)}
    | Eq(lhs, rhs) ->
        {node with Expr = Eq((subst lhs var sub), (subst rhs var sub))}
    | Less(lhs, rhs) ->
        {node with Expr = Less((subst lhs var sub), (subst rhs var sub))}
    | LessEq(lhs, rhs) ->
        {node with Expr = LessEq((subst lhs var sub), (subst rhs var sub))}
    | Greater(lhs, rhs) ->
        {node with Expr = Greater((subst lhs var sub), (subst rhs var sub))}
    | GreaterEq(lhs, rhs) ->
        {node with Expr = GreaterEq((subst lhs var sub), (subst rhs var sub))}

    | SubAssign(lhs, rhs) ->
        {node with Expr = SubAssign((subst lhs var sub), (subst rhs var sub))}
    | AddAssign(lhs, rhs) ->
        {node with Expr = AddAssign((subst lhs var sub), (subst rhs var sub))}
    | DivAssign(lhs, rhs) ->
        {node with Expr = DivAssign((subst lhs var sub), (subst rhs var sub))}
    | MultAssign(lhs, rhs) ->
        {node with Expr = MultAssign((subst lhs var sub), (subst rhs var sub))}
    | RemAssign(lhs, rhs) ->
        {node with Expr = RemAssign((subst lhs var sub), (subst rhs var sub))}

    | ReadInt
    | ReadFloat -> node // The substitution has no effect

    | Print(arg) ->
        {node with Expr = Print(subst arg var sub)}
    | PrintLn(arg) ->
        {node with Expr = PrintLn(subst arg var sub)}
    | Syscall(num, args) ->
        {node with Expr = Syscall(num, List.map (fun n -> (subst n var sub)) args)}
    
    | Preinc(arg) ->
        {node with Expr = Preinc(subst arg var sub)}
    
    | Postinc(arg) ->
        {node with Expr = Postinc(subst arg var sub)}
    
    | If(cond, ifTrue, ifFalse) ->
        {node with Expr = If((subst cond var sub), (subst ifTrue var sub),
                                                   (subst ifFalse var sub))}

    | Seq(nodes) ->
        let substNodes = List.map (fun n -> (subst n var sub)) nodes
        {node with Expr = Seq(substNodes)}

    | Type(tname, def, scope) ->
        {node with Expr = Type(tname, def, (subst scope var sub))}

    | Ascription(tpe, node) ->
        {node with Expr = Ascription(tpe, (subst node var sub))}

    | Assertion(arg) ->
        {node with Expr = Assertion(subst arg var sub)}

    | Copy(arg) ->
        {node with Expr = Copy(subst arg var sub)}

    | Let(vname, init, scope) when vname = var ->
        // The variable is shadowed, do not substitute it in the "let" scope
        {node with Expr = Let(vname, (subst init var sub), scope)}
    | Let(vname, init, scope) ->
        // Propagate the substitution in the "let" scope
        {node with Expr = Let(vname, (subst init var sub),
                              (subst scope var sub))}

    | LetT(vname, tpe, init, scope) when vname = var ->
        // The variable is shadowed, do not substitute it in the "let" scope
        {node with Expr = LetT(vname, tpe, (subst init var sub), scope)}
    | LetT(vname, tpe, init, scope) ->
        // Propagate the substitution in the "let" scope
        {node with Expr = LetT(vname, tpe, (subst init var sub),
                               (subst scope var sub))}

    | LetMut(vname, init, scope) when vname = var ->
        // Do not substitute the variable in the "let mutable" scope
        {node with Expr = LetMut(vname, (subst init var sub), scope)}
    | LetMut(vname, init, scope) ->
        {node with Expr = LetMut(vname, (subst init var sub),
                                 (subst scope var sub))}

    | Assign(target, expr) ->
        {node with Expr = Assign((subst target var sub), (subst expr var sub))}

    | While(cond, body) ->
        let substCond = subst cond var sub
        let substBody = subst body var sub
        {node with Expr = While(substCond, substBody)}

    | For(ident, init, cond, step, body) ->
        let substInit = subst init var sub
        let substCond = subst cond var sub
        let substStep = subst step var sub
        let substBody = subst body var sub
        {node with Expr = For(ident, substInit, substCond, substStep, substBody)}

    | Lambda(args, body) ->
        /// Arguments of this lambda term, without their pretypes
        let (argVars, _) = List.unzip args
        if (List.contains var argVars) then node // No substitution
        else {node with Expr = Lambda(args, (subst body var sub))}

    | Application(expr, args) ->
        let substExpr = subst expr var sub
        let substArgs = List.map (fun n -> (subst n var sub)) args
        {node with Expr = Application(substExpr, substArgs)}

    | StructCons(fields) ->
        let (fieldMutables, fieldNames, initNodes) = List.unzip3 fields
        let substInitNodes = List.map (fun e -> (subst e var sub)) initNodes
        {node with Expr = StructCons(List.zip3 fieldMutables fieldNames substInitNodes)}

    | FieldSelect(target, field) ->
        {node with Expr = FieldSelect((subst target var sub), field)}

    | UnionCons(label, expr) ->
        {node with Expr = UnionCons(label, (subst expr var sub))}

    | Match(expr, cases) ->
        /// Mapper function to propagate the substitution along a match case
        let substCase(lab: string, v: string, cont: Node<'E,'T>) =
            if (v = var) then (lab, v, cont) // Variable bound, no substitution
            else (lab, v, (subst cont var sub))
        let cases2 = List.map substCase cases
        {node with Expr = Match((subst expr var sub), cases2)}
    | Array(length, data) ->
        {node with Expr = Array((subst length var sub), (subst data var sub))}
    | ArrayLength(arr) ->
        {node with Expr = ArrayLength(subst arr var sub)}
    | ArrayElem(arr, index) ->
        {node with Expr = ArrayElem((subst arr var sub), (subst index var sub))}

/// Compute the set of free variables in the given AST node.
let rec freeVars (node: Node<'E,'T>): Set<string> =
    match node.Expr with
    | UnitVal
    | IntVal(_)
    | BoolVal(_)
    | FloatVal(_)
    | StringVal(_)
    | Pointer(_) -> Set[]
    | Var(name) -> Set[name]
    | Add(lhs, rhs)
    | Mult(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | Div(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | BAnd(lhs, rhs)
    | BOr(lhs, rhs)
    | BXor(lhs, rhs)
    | BSL(lhs, rhs)
    | BSR(lhs, rhs)
    | Rem(lhs, rhs) 
    | BAnd(lhs, rhs)
    | BOr(lhs, rhs)
    | BXor(lhs, rhs)
    | BSL(lhs, rhs)
    | BSR(lhs, rhs)
    | Sub(lhs, rhs)
    | And(lhs, rhs)
    | ScAnd(lhs, rhs)
    | Xor(lhs, rhs)
    | ScOr(lhs, rhs)
    | Or(lhs, rhs) 
    | AddAssign(lhs, rhs)
    | SubAssign(lhs, rhs)
    | DivAssign(lhs, rhs)
    | MultAssign(lhs, rhs) 
    | RemAssign(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | BNot(arg)
    | Sqrt(arg) -> freeVars arg
    | Not(arg) -> freeVars arg
    | Neg(arg) -> freeVars arg
    | Eq(lhs, rhs)
    | Less(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | LessEq(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | Greater(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | GreaterEq(lhs, rhs) ->
        Set.union (freeVars lhs) (freeVars rhs)
    | ReadInt
    | ReadFloat -> Set[]
    | Print(arg)
    | PrintLn(arg) -> freeVars arg
    | Preinc(arg) -> freeVars arg
    | Postinc(arg) -> freeVars arg
    | If(condition, ifTrue, ifFalse) ->
        Set.union (freeVars condition)
                  (Set.union (freeVars ifTrue) (freeVars ifFalse))
    | Seq(nodes) -> freeVarsInList nodes
    | Ascription(_, node) -> freeVars node
    | Let(name, init, scope)
    | LetT(name, _, init, scope)
    | LetMut(name, init, scope) ->
        // All the free variables in the 'let' initialisation, together with all
        // free variables in the scope --- minus the newly-bound variable
        Set.union (freeVars init) (Set.remove name (freeVars scope))
    | Assign(target, expr) ->
        // Union of the free names of the lhs and the rhs of the assignment
        Set.union (freeVars target) (freeVars expr)
    | While(cond, body) -> Set.union (freeVars cond) (freeVars body)
    | For(ident, init, cond, step, body) -> Set.union (freeVars cond) (freeVars body)
    | Assertion(arg) -> freeVars arg
    | Syscall(_, args) -> freeVarsInList args
    | Copy(arg) -> freeVars arg
    | Type(_, _, scope) -> freeVars scope
    | Lambda(args, body) ->
        let (argNames, _) = List.unzip args
        // All the free variables in the lambda function body, minus the
        // names of the arguments
        Set.difference (freeVars body) (Set.ofList argNames)
    | Application(expr, args) ->
        let fvArgs = List.map freeVars args
        // Union of free variables in the applied expr, plus all its arguments
        Set.union (freeVars expr) (freeVarsInList args)
    | StructCons(fields) ->
        let (_, _, nodes) = List.unzip3 fields
        freeVarsInList nodes
    | FieldSelect(expr, _) -> freeVars expr
    | UnionCons(_, expr) -> freeVars expr
    | Match(expr, cases) ->
        /// Compute the free variables in all match cases continuations, minus
        /// the variable bound in the corresponding match case.  This 'folder'
        /// is used to fold over all match cases.
        let folder (acc: Set<string>) (_, var, cont: Node<'E,'T>): Set<string> =
            Set.union acc ((freeVars cont).Remove var)
        /// Free variables in all match continuations
        let fvConts = List.fold folder Set[] cases
        Set.union (freeVars expr) fvConts
    | Array(length, data) ->
        Set.union (freeVars length) (freeVars data)
    | ArrayLength(arr) ->
        freeVars arr
    | ArrayElem(arr, index) ->
        Set.union (freeVars arr) (freeVars index)

/// Compute the union of the free variables in a list of AST nodes.
and internal freeVarsInList (nodes: List<Node<'E,'T>>): Set<string> =
    /// Compute the free variables of 'node' and add them to the accumulator
    let folder (acc: Set<string>) (node: Node<'E,'T> ) =
        Set.union acc (freeVars node)
    List.fold folder Set[] nodes


/// Compute the set of captured variables in the given AST node.
let rec capturedVars (node: Node<'E,'T>): Set<string> =
    match node.Expr with
    | UnitVal
    | IntVal(_)
    | BoolVal(_)
    | FloatVal(_)
    | StringVal(_)
    | Pointer(_)
    | Lambda(_, _) ->
        // All free variables of a value are considered as captured
        freeVars node
    | Var(_) -> Set[]
    | BAnd(lhs, rhs)
    | BOr(lhs, rhs)
    | BXor(lhs, rhs)
    | BSL(lhs, rhs)
    | BSR(lhs, rhs)
    | Sub(lhs, rhs)
    | Add(lhs, rhs)
    | Mult(lhs, rhs)
    | AddAssign(lhs, rhs)
    | SubAssign(lhs, rhs) 
    | MultAssign(lhs, rhs)
    | DivAssign(lhs, rhs)
    | RemAssign(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | Div(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | Rem(lhs, rhs) 
    | And(lhs, rhs)
    | ScAnd(lhs, rhs)
    | Xor(lhs, rhs)
    | ScOr(lhs, rhs)
    | Or(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | BNot(arg)
    | Sqrt(arg) -> capturedVars arg
    | Not(arg) -> capturedVars arg
    | Neg(arg) -> capturedVars arg
    | Eq(lhs, rhs)
    | Less(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | LessEq(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | Greater(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | GreaterEq(lhs, rhs) ->
        Set.union (capturedVars lhs) (capturedVars rhs)
    | ReadInt
    | ReadFloat -> Set[]
    | Print(arg)
    | PrintLn(arg) -> capturedVars arg
    | Preinc(arg) -> capturedVars arg
    | Postinc(arg) -> capturedVars arg
    | If(condition, ifTrue, ifFalse) ->
        Set.union (capturedVars condition)
                  (Set.union (capturedVars ifTrue) (capturedVars ifFalse))
    | Seq(nodes) -> capturedVarsInList nodes
    | Ascription(_, node) -> capturedVars node
    | Let(name, init, scope)
    | LetT(name, _, init, scope)
    | LetMut(name, init, scope) ->
        // All the captured variables in the 'let' initialisation, together with
        // all captured variables in the scope --- minus the newly-bound var
        Set.union (capturedVars init) (Set.remove name (capturedVars scope))
    | Assign(target, expr) ->
        // Union of the captured vars of the lhs and the rhs of the assignment
        Set.union (capturedVars target) (capturedVars expr)
    | While(cond, body) -> Set.union (capturedVars cond) (capturedVars body)
    | For(ident, init, cond, step, body) -> Set.union (capturedVars cond) (capturedVars body)
    | Assertion(arg) -> capturedVars arg
    | Syscall(_, args) -> capturedVarsInList args
    | Copy(arg) -> capturedVars arg
    | Type(_, _, scope) -> capturedVars scope
    | Application(expr, args) ->
        let fvArgs = List.map capturedVars args
        // Union of captured variables in the applied expr, plus all arguments
        Set.union (capturedVars expr) (capturedVarsInList args)
    | StructCons(fields) ->
        let (_, _, nodes) = List.unzip3 fields
        capturedVarsInList nodes
    | FieldSelect(expr, _) -> capturedVars expr
    | UnionCons(_, expr) -> capturedVars expr
    | Match(expr, cases) ->
        /// Compute the captured variables in all match cases continuations,
        /// minus the variable bound in the corresponding match case.  This
        /// 'folder' is used to fold over all match cases.
        let folder (acc: Set<string>) (_, var, cont: Node<'E,'T>): Set<string> =
            Set.union acc ((capturedVars cont).Remove var)
        /// Captured variables in all match continuations
        let cvConts = List.fold folder Set[] cases
        Set.union (capturedVars expr) cvConts
    | Array(length, data) ->
        Set.union (capturedVars length) (capturedVars data)
    | ArrayLength(arr) ->
        capturedVars arr
    | ArrayElem(arr, index) ->
        Set.union (capturedVars arr) (capturedVars index)

/// Compute the union of the captured variables in a list of AST nodes.
and internal capturedVarsInList (nodes: List<Node<'E,'T>>): Set<string> =
    /// Compute the free variables of 'node' and add them to the accumulator
    let folder (acc: Set<string>) (node: Node<'E,'T> ) =
        Set.union acc (capturedVars node)
    List.fold folder Set[] nodes
