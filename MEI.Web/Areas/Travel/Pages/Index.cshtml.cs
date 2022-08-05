using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Areas.Travel.Pages
{
    public class IndexModel : PageModel
    {
        public SpacingModel Spacing { get; set; }

        public IList<AssignedTask> AssignedTasks { get; set; }

        public List<SplineAreaChartData> DemoChartData { get; set; } = new List<SplineAreaChartData>();

        public string Client1Avg { get; set; }

        public string Client2Avg { get; set; }

        public string Client3Avg { get; set; }


        public IActionResult OnGet()
        {
            Spacing = new SpacingModel();
            Spacing.CellSpacing = new double[] {30, 30};

            // Get chart data
            DemoChartData = new List<SplineAreaChartData>
            {
                new SplineAreaChartData {xValue = "JAN", yValue = 74.9, yValue1 = 15, yValue2 = 10.8},
                new SplineAreaChartData {xValue = "FEB", yValue = 40.2, yValue1 = 55.5, yValue2 = 21.3},
                new SplineAreaChartData {xValue = "MAR", yValue = 65.8, yValue1 = 17.5, yValue2 = 11.1},
                new SplineAreaChartData {xValue = "APR", yValue = 80.0, yValue1 = 45.5, yValue2 = 21.6},
                new SplineAreaChartData {xValue = "MAY", yValue = 70.8, yValue1 = 65.8, yValue2 = 12},
                new SplineAreaChartData {xValue = "JUN", yValue = 44.6, yValue1 = 81.7, yValue2 = 21.7},
                new SplineAreaChartData {xValue = "JUL", yValue = 74.9, yValue1 = 15, yValue2 = 10.8},
                new SplineAreaChartData {xValue = "AUG", yValue = 40.2, yValue1 = 55.5, yValue2 = 21.3},
                new SplineAreaChartData {xValue = "SEP", yValue = 65.8, yValue1 = 17.5, yValue2 = 11.1},
                new SplineAreaChartData {xValue = "OCT", yValue = 92.0, yValue1 = 45.5, yValue2 = 21.6},
                new SplineAreaChartData {xValue = "NOV", yValue = 70.8, yValue1 = 65.8, yValue2 = 12},
                new SplineAreaChartData {xValue = "DEC", yValue = 65.6, yValue1 = 81.7, yValue2 = 21.7}
            };

            // Get chart Averages
            var avg1 = DemoChartData.Sum(c => c.yValue) / 12;
            var avg2 = DemoChartData.Sum(c => c.yValue1) / 12;
            var avg3 = DemoChartData.Sum(c => c.yValue2) / 12;
            Client1Avg = $"{(long) avg1}%";
            Client2Avg = $"{(long) avg2}%";
            Client3Avg = $"{(long) avg3}%";

            AssignedTasks = AssignedTask.Get();

            return Page();
        }
    }

    public class SplineAreaChartData
    {
        public string xValue;
        public double yValue;
        public double yValue1;
        public double yValue2;
    }

    public class SpacingModel
    {
        public double[] CellSpacing { get; set; }
    }

    public class AssignedTask
    {
        public int Id { get; set; }

        public string ConsultantName { get; set; }

        public string LocationName { get; set; }

        public string ProgramGuid { get; set; }

        public string Status { get; set; }

        public string Title => GetTitle();

        public string DaysOutMessage { get; set; }

        private string GetTitle()
        {
            return $"{ConsultantName} - {LocationName} - {ProgramGuid}";
        }

        public static IList<AssignedTask> Get()
        {
            return new List<AssignedTask>
            {
                new AssignedTask
                {
                    Id = 1,
                    ConsultantName = "Julie Watherly",
                    LocationName = "Ruth's Chris Steak House",
                    ProgramGuid = "DEF04-DM01-19",
                    Status = "Program Changes",
                    DaysOutMessage = "11 Days Out"
                },
                new AssignedTask
                {
                    Id = 2,
                    ConsultantName = "Michael Schmidt",
                    LocationName = "Flemming’s Prime Steakhouse",
                    ProgramGuid = "DEE02-DM02-19",
                    Status = "Outreach",
                    DaysOutMessage = "13 Days Out"
                },
                new AssignedTask
                {
                    Id = 3,
                    ConsultantName = "Matt Overton",
                    LocationName = "Office of Dr. Overton",
                    ProgramGuid = "ILL03-GO02-19",
                    Status = "Invoiced",
                    DaysOutMessage = "20 Days Out"
                },
                new AssignedTask
                {
                    Id = 4,
                    ConsultantName = "Debra Baggins",
                    LocationName = "Biaggi’s Italiano",
                    ProgramGuid = "ILK34-GD01-19",
                    Status = "Speaker Confirmation",
                    DaysOutMessage = "21 Days Out"
                },
                new AssignedTask
                {
                    Id = 5,
                    ConsultantName = "Micheal Rathers",
                    LocationName = "Brew Brother’s Taphouse",
                    ProgramGuid = "DEF05-DM02-19",
                    Status = "Outreach",
                    DaysOutMessage = "32 Days Out"
                }
                //new AssignedTask {Id = 6, ConsultantName = "Julie Watherly", LocationName = "Ruth's Chris Steak House", ProgramGuid = "DEF04-DM02-19", Status = "Program Changes", DaysOutMessage = "11 Days Out"},
                //new AssignedTask {Id = 7, ConsultantName = "Michael Schmidt", LocationName = "Flemming’s Prime Steakhouse", ProgramGuid = "DEE02-DM03-19", Status = "Outreach", DaysOutMessage = "13 Days Out"},
                //new AssignedTask {Id = 8, ConsultantName = "Matt Overton", LocationName = "Office of Dr. Overton", ProgramGuid = "ILL03-GO03-19", Status = "Invoiced", DaysOutMessage = "20 Days Out"},
                //new AssignedTask {Id = 9, ConsultantName = "Debra Baggins", LocationName = "Biaggi’s Italiano", ProgramGuid = "ILK34-GD04-19", Status = "Speaker Confirmation", DaysOutMessage = "21 Days Out"},
                //new AssignedTask {Id = 10, ConsultantName = "Micheal Rathers", LocationName = "Brew Brother’s Taphouse", ProgramGuid = "DEF05-DM02-19", Status = "Outreach", DaysOutMessage = "32 Days Out"},
            };
        }
    }
}