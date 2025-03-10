using System;
using System.Collections.Generic;
using System.Globalization; 
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;

namespace DotVVM.Samples.BasicSamples.ViewModels
{
    public class Sample14ViewModel : DotvvmViewModelBase
    {
        public override Task Init()
        {
            if (!Context.IsPostBack)
            {
                Months = new List<MonthData>();
                for (int i = 0; i < 12; i++)
                {
                    Months.Add(new MonthData()
                    {
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i]
                    });
                } 
            }

            return base.Init();
        }


        public List<MonthData> Months { get; set; }

        public void Recalculate()
        {
            TotalHours = Months.Sum(m => m.Hours);
        }

        public int TotalHours { get; set; }
    }

    public class MonthData
    {
        public string MonthName { get; set; }

        public int Hours { get; set; }
    }
}