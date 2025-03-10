using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Binding;

namespace DotVVM.Framework.Runtime.Filters
{
    public class ActionInfo
    {
        public BindingExpression Binding { get; set; }
        public bool IsControlCommand { get; internal set; }

        public Action Action { get; set; }
    }
}