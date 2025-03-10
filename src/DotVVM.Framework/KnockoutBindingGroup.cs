using System;
using System.Collections.Generic;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;
using Newtonsoft.Json;

namespace DotVVM.Framework
{
    public class KnockoutBindingGroup
    {

        private List<KnockoutBindingInfo> info = new List<KnockoutBindingInfo>();
        
        public bool IsEmpty => info.Count == 0;

        public void Add(string name, DotvvmBindableControl control, DotvvmProperty property, Action nullBindingAction)
        {
            var binding = control.GetValueBinding(property);
            if (binding == null)
            {
                nullBindingAction();
            }
            else
            {
                info.Add(new KnockoutBindingInfo() { Name = name, Expression = control.GetValueBinding(property).GetKnockoutBindingExpression() });
            }
        }

        public void Add(string name, string expression, bool surroundWithDoubleQuotes = false)
        {
            if (surroundWithDoubleQuotes)
            {
                expression = JsonConvert.SerializeObject(expression);
            }

            info.Add(new KnockoutBindingInfo() { Name = name, Expression = expression });
        }
        


        public override string ToString()
        {
            return "{ " + string.Join(", ", info) + " }";
        }


        class KnockoutBindingInfo
        {
            public string Name { get; set; }
            public string Expression { get; set; }

            public override string ToString()
            {
                return "\"" + Name + "\": " + Expression;
            }
        }

    }
}