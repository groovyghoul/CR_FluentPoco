using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;

namespace dxFluentPoco
{
    public partial class FluentPocoPlugIn : StandardPlugIn
    {
        // DXCore-generated code...
        #region InitializePlugIn
        public override void InitializePlugIn()
        {
            base.InitializePlugIn();

            RegisterFluentPoco();
        }
        #endregion
        #region FinalizePlugIn
        public override void FinalizePlugIn()
        {
            //
            // TODO: Add your finalization code here.
            //

            base.FinalizePlugIn();
        }
        #endregion

        public void RegisterFluentPoco()
        {
            CodeProvider FluentPoco = new CodeProvider(components);
            ((ISupportInitialize)(FluentPoco)).BeginInit();
            FluentPoco.AutoUndo = true;
            FluentPoco.ProviderName = "CR_FluentPoco"; // Should be Unique
            FluentPoco.DisplayName = "Create Fluent Poco Stuff";
            FluentPoco.Description = "Turns a list of private fields into fluent methods.";
            FluentPoco.CheckAvailability += FluentPoco_CheckAvailability;
            FluentPoco.Apply += FluentPoco_Apply;
            ((ISupportInitialize)(FluentPoco)).EndInit();
        }

        private void FluentPoco_CheckAvailability(Object sender, CheckContentAvailabilityEventArgs ea)
        {
            Member element = CodeRush.Source.ActiveMember as Member;
            if (element != null && element is IFieldElement && element.Visibility != MemberVisibility.Public )
            {
                ea.Available = true;
            }
        }

        private void FluentPoco_Apply(Object sender, ApplyContentEventArgs ea)
        {
            Class activeClass = CodeRush.Source.ActiveClass;

            foreach (var fld in activeClass.AllFields)
            {
                BuildGetterProperty(activeClass, fld as Member);
                BuildSetterMethod(activeClass, fld as Member);
            }

            BuildFluentStartingPoint(activeClass);
        }

        private void BuildGetterProperty(Class activeClass, Member element)
        {
            var builder = new ElementBuilder();

            var publicName = char.ToUpper(element.Name[0]) + element.Name.Substring(1);

            Property newProperty = builder.AddProperty(null, element.GetTypeName(), publicName);
            newProperty.Visibility = MemberVisibility.Public;
            Get getter = builder.AddGetter(newProperty);
            builder.AddReturn(getter, element.Name);

            var generatedProperty = builder.GenerateCode();

            int LastLine = activeClass.Range.End.Line;

            SourcePoint InsertionPoint = new SourcePoint(LastLine, 1);

            var newPropertyRange = CodeRush.Documents.ActiveTextDocument.InsertText(InsertionPoint, Environment.NewLine + generatedProperty);

            CodeRush.Documents.Format(newPropertyRange);
        }

        private void BuildSetterMethod(Class activeClass, Member element)
        {
            var builder = new ElementBuilder();

            var publicName = char.ToUpper(element.Name[0]) + element.Name.Substring(1);

            var method = builder.AddMethod(activeClass, activeClass.Name, "With" + publicName);
            method.Visibility = MemberVisibility.Public;
            method.Parameters.Add(new Param(element.GetTypeName(), element.Name));
            method.AddNode(builder.BuildAssignment("this." + element.Name, element.Name));
            method.AddNode(builder.BuildReturn("this"));

            activeClass.AddNode(method);
            var Code = CodeRush.CodeMod.GenerateCode(method, false);

            var LastLine = activeClass.Range.End.Line;

            var InsertionPoint = new SourcePoint(LastLine, 1);

            var newMethodRange = CodeRush.Documents.ActiveTextDocument.InsertText(InsertionPoint, Environment.NewLine + Code);

            CodeRush.Documents.Format(newMethodRange);

        }

        private void BuildFluentStartingPoint(Class activeClass)
        {
            var builder = new ElementBuilder();
            var newClass = builder.AddClass(null, activeClass.Name + "Helper");
            newClass.Visibility = MemberVisibility.Public;

            var method = builder.AddMethod(newClass, activeClass.Name, "Create");
            method.Visibility = MemberVisibility.Public;
            method.IsStatic = true;
            method.AddNode(builder.BuildReturn(String.Format("new {0}()", activeClass.Name)));
            newClass.AddNode(method);

            var generatedClass = builder.GenerateCode();

            int LastLine = activeClass.Range.End.Line;

            SourcePoint InsertionPoint = new SourcePoint(LastLine + 1, 1);

            var newClassRange = CodeRush.Documents.ActiveTextDocument.InsertText(InsertionPoint, Environment.NewLine + generatedClass);

            CodeRush.Documents.Format(newClassRange);
        }
    }
}