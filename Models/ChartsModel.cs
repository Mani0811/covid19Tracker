using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Models
{
    public class ChartsModel
    {
        public List<string> categories { get; set; }
        public List<Series> allSeries { get; set; }

        public string categoriesString { get; set; }
        public string allSeriesString { get; set; }

        public List<CasesTimeSery> cases_time_series { get; set; }
        public List<KeyValue> key_values { get; set; }
        public List<Statewise> statewise { get; set; }
        public List<Tested> tested { get; set; }
    }

    public class Series
    {
        public string name { get; set; }
        public List<int?> data { get; set; }        
    }

    public class ChartsModelMapper
    {
        public static ChartsModel Map(NationDataModel nationDataModel)
        {
            ChartsModel chartsModel = new ChartsModel
            {
                cases_time_series = nationDataModel.cases_time_series,
                key_values = nationDataModel.key_values,
                statewise = nationDataModel.statewise,
                tested = nationDataModel.tested,
                categories = MapCategories(nationDataModel.cases_time_series),
                allSeries = MapSeries(nationDataModel.cases_time_series),
            };

            chartsModel.categoriesString = JsonConvert.SerializeObject(chartsModel.categories);
            chartsModel.allSeriesString = JsonConvert.SerializeObject(chartsModel.allSeries);
            return chartsModel;
        }

        private static List<Series> MapSeries(List<CasesTimeSery> cases_time_series)
        {
            Series dailyConfirmed = new Series();
            dailyConfirmed.data = new List<int?>();

            Series dailyDeceased = new Series();
            dailyDeceased.data = new List<int?>();

            Series dailyRecovered = new Series();
            dailyRecovered.data = new List<int?>();

            Series death = new Series();
            death.data = new List<int?>();

            Series rec = new Series();
            rec.data = new List<int?>();

            Series totalConfirmed = new Series();
            totalConfirmed.data = new List<int?>();

            Series totalDeceased = new Series();
            totalDeceased.data = new List<int?>();

            Series totalRecovered = new Series();
            totalRecovered.data = new List<int?>();

            List<Series> all = new List<Series>();

            foreach (var item in cases_time_series)
            {
                dailyDeceased.data.Add(ParseToNullableInt(item.dailydeceased));
                dailyRecovered.data.Add(ParseToNullableInt(item.dailyrecovered));
                death.data.Add(ParseToNullableInt(item.death));
                rec.data.Add(ParseToNullableInt(item.rec));
                totalConfirmed.data.Add(ParseToNullableInt(item.totalconfirmed));
                totalDeceased.data.Add(ParseToNullableInt(item.totaldeceased));
                totalRecovered.data.Add(ParseToNullableInt(item.totalrecovered));
            }

            //all.Add(dailyConfirmed);
            //all.Add(dailyDeceased);
            //all.Add(dailyRecovered);
            //all.Add(death);
            //all.Add(rec);
            all.Add(totalConfirmed);
            all.Add(totalDeceased);
            all.Add(totalRecovered);

            return all;
        }

        public static int? ParseToNullableInt(string value)
        {
            return String.IsNullOrEmpty(value) ? null : (int.Parse(value) as int?);
        }

        public static List<String> MapCategories(List<CasesTimeSery> casesTimeSery)
        {
            List<string> categories = new List<string>();

            for (int i = 0; i < casesTimeSery.Count; i++)
            {
                categories.Add(casesTimeSery[i].date.Trim());
            }

            return categories;
        }
    }

}
