using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Skyrim_Interpreter;
using JetBrains.Annotations;
using UnityEditor;

namespace Assets.Scripts.Interpreter
{
    public class CodeGenerator
    {
        private List<Effect> effectnode;

        public CodeGenerator(List<Effect> effects)
        {
            effectnode = effects;
        }


        public void Generate(string output)
        {
            using (StreamWriter writer = new StreamWriter(output))
            {
                writer.Write("using System; \n using System.Linq; \n using Skyrim_Interpreter;");
                writer.WriteLine();
                writer.WriteLine("public class EffectGenerator \n{");



                foreach (Effect effect in effectnode)
                {
                    GenerateMethod(writer, effect);
                }

                writer.WriteLine(" } ");

            }
        }

        private void GenerateMethod(StreamWriter writer, Effect effect)
        {
            string parameter;
            if (effect.Parameters.Count != 0)
            {
                var parameters = new List<string>();
                foreach (var item in effect.Parameters)
                {
                    if (item is not ColonASTNode colon) { throw new Exception("Los parametros estan mal"); }
                    if (colon.left is not IdentifierASTNode id) { throw new Exception("El hijo izquierdo esta mal"); }
                    Type tipo = GameContext.Parameters[id.value].GetType();
                    if (tipo is int)
                    {
                        parameters.Add($"int {id.value}");
                    }
                    else if (tipo is string)
                    {
                        parameters.Add($"string {id.value}");
                    }
                    else if (tipo is bool)
                    {
                        parameters.Add($"bool {id.value}");
                    }
                }
                parameter = "," + string.Join(",", parameters);
            }
            else
            {
                parameter = "";
            }

            writer.WriteLine($"    public void {effect.Name}(List<Card> target,Context context {parameter})");
            writer.WriteLine("    {");
            writer.WriteLine("         UnityEngine.Debug.Log(\"EffectoEjecutado\");");
            writer.WriteLine("          UnityEngine.Debug.Log(target.Count());");
            writer.WriteLine("         UnityEngine.Debug.Log(\"Current:\" + GameManager.Instancia.CurrentPlayer);");

            string hallo = "";
            foreach (var action in effect.Action.actions)
            {
                hallo += GenerateAction(writer, action);
            }
            hallo += "\n";
            writer.WriteLine(hallo);
        }

        private string GenerateAction(StreamWriter writer, ASTnode action)
        {
            if (action is AssignASTNode assign)
            {
                if (assign.Left is not IdentifierASTNode id) throw new Exception("Esta mal");
                string typeofvariable = GameContext.Assignment.ContainsKey(id.value) ? "" : "var";
                return $"\n{typeofvariable} {id.value} = {GenerateAction(writer, assign.Right)}";
            }
            else if (action is AccessASTNode access)
            {
                string first = "", last = "";
                if (access.left is IdentifierASTNode i1)
                {
                    first = i1.value;
                }
                else if (access.left is AccessASTNode acc)
                {
                    first = GenerateAction(writer, acc);
                }
                else { throw new Exception("Esta mal el hijo izquiedo de acces"); }

                if (access.right is IdentifierASTNode i2)
                {
                    string parameter = "";
                    if (i2.Have)
                    {
                        if (i2.Parameters != null && i2.Parameters is IdentifierASTNode ii) parameter = $"( var {ii.value})";
                        else parameter = "();";
                    }
                    last = i2.value + parameter;
                }
                else if (access.right is AccessASTNode a)
                {
                    last = GenerateAction(writer, a);
                }
                else throw new Exception("Esta mal el hijo derecho del access");
                return $"\n {first}.{last}";
            }
            else if (action is WhileASTNode whil)
            {
                ASTnode condition = whil.condition;
                string mycondition = GenerateAction(writer, condition);
                Debug.Log($"La condiciooooooooonnnnnnnnnnnn eeeeeeeessssssssssss {mycondition}");
                string body = "";
                foreach (var item in whil.block.Block.param)
                {
                    body += GenerateAction(writer, item);
                }
                string w = "";
                w += $"\n  While({mycondition})";
                w += "{\n";
                w += $"{body}";
                w += "\n}";
                return w;
            }
            else if (action is LiteralASTNode literal)
            {
                return $"{literal.value}";
            }
            else if (action is ComparationASTNode comp)
            {
                if (comp.type == Token_Type.LESS) { return $"\n{GenerateAction(writer, comp.left)} < {GenerateAction(writer, comp.right)}"; }
                if (comp.type == Token_Type.LESS_EQUAL) { return $"\n{GenerateAction(writer, comp.left)} <= {GenerateAction(writer, comp.right)}"; }
                if (comp.type == Token_Type.GREATER) { return $"\n{GenerateAction(writer, comp.left)} > {GenerateAction(writer, comp.right)}"; }
                if (comp.type == Token_Type.GREATER_EQUAL) { return $"\n{GenerateAction(writer, comp.left)} >= {GenerateAction(writer, comp.right)}"; }
            }
            else if (action is EqualASTNode || action is NotEqualASTNode)
            {
                if (action is EqualASTNode e) return $"\n{GenerateAction(writer, e.Left)} =={GenerateAction(writer, e.Right)} ";
                else if (action is NotEqualASTNode n) return $"\n{GenerateAction(writer, n.Left)} != {GenerateAction(writer, n.Right)}";
            }
            else if (action is MinusASTNode || action is PlusAST)
            {
                if (action is MinusASTNode m) return $"{GenerateAction(writer, m.LeftChild)} - {GenerateAction(writer, m.RightChild)}";
                else if (action is PlusAST p) return $"{GenerateAction(writer, p.LeftChild)} + {GenerateAction(writer, p.RightChild)}";
            }
            else if (action is ConcatenationASTNode con)
            {
                return $"{con.left} + {con.right}";
            }
            else if (action is SemicolomASTNode)
            {
                return ";\n";
            }
            else if (action is FactorASTNode fac)
            {
                if (fac.type == Token_Type.MULTIPLY) return $"{GenerateAction(writer, fac.leftchild)} * {GenerateAction(writer, fac.rightchild)}";
                if (fac.type == Token_Type.DIVIDE) return $"{GenerateAction(writer, fac.leftchild)} / {GenerateAction(writer, fac.rightchild)}";
                if (fac.type == Token_Type.MODULUS) return $"{GenerateAction(writer, fac.leftchild)} % {GenerateAction(writer, fac.rightchild)}";
            }
            else if (action is ForASTNode f)
            {
                string t = "";
                t += "foreach (var item in target){\n";
                foreach (var item in f.block.Block.param)
                {
                    t += $"{GenerateAction(writer, item)}";
                }
                t += "}\n";
            }
            else if (action is AssgnWithValueASTNode asi)
            {
                if (asi.value == "+=")
                    return $"{GenerateAction(writer, asi.left)} += {GenerateAction(writer, asi.right)}";
                else
                    return $"{GenerateAction(writer, asi.left)} += {GenerateAction(writer, asi.right)}";
            }
            else if (action is IdentifierASTNode i)
            {
                string typeofvariable = GameContext.Assignment.ContainsKey(i.value) ? "" : "var";
                return $"{typeofvariable} {i.value}";
            }
            else if (action is UnaryASTNode u)
            {
                if (u.value == "++") return $"{u.Son}++";
                else if (u.value == "--") return $"{u.Son}--";
                else return $"!{u.Son}";
            }
            else if (action is GroupingASTNode grup) 
            {
                return $"({GenerateAction(writer,grup.groupnode)})";
            }
            return "";
        }
    }
}
