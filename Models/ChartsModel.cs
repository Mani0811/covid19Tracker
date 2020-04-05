using AspNetCoreTodo.Models;
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
        public List<StateDistrictModel> stateDistrictModels { get; set; }
    }
}

public class Series
{
    public string name { get; set; }
    public List<int?> data { get; set; }
}

public class ChartsModelMapper
{
    public static ChartsModel Map(NationDataModel nationDataModel, List<StateDistrictModel> stateDistrictModelList)
    {

        ChartsModel chartsModel = new ChartsModel
        {
            cases_time_series = nationDataModel.cases_time_series,
            key_values = nationDataModel.key_values,
            statewise = MapStateWise(nationDataModel.statewise, stateDistrictModelList),
            tested = nationDataModel.tested,

            categories = MapCategories(nationDataModel.cases_time_series),
            allSeries = MapSeries(nationDataModel.cases_time_series),

            stateDistrictModels = stateDistrictModelList
        };

        chartsModel.categoriesString = JsonConvert.SerializeObject(chartsModel.categories);
        chartsModel.allSeriesString = JsonConvert.SerializeObject(chartsModel.allSeries);
        return chartsModel;
    }

    private static List<Statewise> MapStateWise(List<Statewise> statewiseList, List<StateDistrictModel> stateDistrictModel)
    {
        var map = new Dictionary<string, List<DistrictData>>();
        foreach (var item in stateDistrictModel)
        {
            map.Add(item.state, item.districtData);
        }

        foreach (var item in statewiseList)
        {
            item.delta = MapStateDelta(item.delta, item);
            item.districtData = map.ContainsKey(item.state) ? map[item.state] : new List<DistrictData>();
        }
        return statewiseList;
    }

    private static Delta MapStateDelta(Delta delta, Statewise item)
    {
        return new Delta
        {
           // active = ParseToNullableInt(item.delta.active),
            confirmed = ParseToNullableInt(item?.deltaconfirmed),
            deaths = ParseToNullableInt(item?.deltadeaths),
            recovered = ParseToNullableInt(item?.deltarecovered)
        };
    }

    private static List<Series> MapSeries(List<CasesTimeSery> cases_time_series)
    {
        Series dailyConfirmed = new Series
        {
            data = new List<int?>(),
            name = "Daily Confirmed"
        };

        Series dailyDeceased = new Series
        {
            data = new List<int?>(),
            name = "Daily Deaths"
        };

        Series dailyRecovered = new Series
        {
            data = new List<int?>(),
            name = "Daily Recovered"
        };

        Series death = new Series
        {
            data = new List<int?>(),
            name = "death"
        };

        Series rec = new Series
        {
            data = new List<int?>(),
            name = "rec"
        };

        Series totalConfirmed = new Series
        {
            data = new List<int?>(),
            name = "Total Confirmed"
        };

        Series totalDeceased = new Series
        {
            data = new List<int?>(),
            name = "Total Deaths"
        };

        Series totalRecovered = new Series
        {
            data = new List<int?>(),
            name = "Total Recovered"
        };

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
        all.Add(dailyDeceased);
        all.Add(dailyRecovered);
        //all.Add(death);
        //all.Add(rec);
        all.Add(totalConfirmed);
        all.Add(totalDeceased);
        all.Add(totalRecovered);

        return all;
    }

    public static int? ParseToNullableInt(string value)
    {
        return int.TryParse(value, out int intergerValue) ? intergerValue : (int?)null;
    }

    public static List<string> MapCategories(List<CasesTimeSery> casesTimeSery)
    {
        List<string> categories = new List<string>();

        for (int i = 0; i < casesTimeSery.Count; i++)
        {
            categories.Add(casesTimeSery[i].date.Trim());
        }

        return categories;
    }
}


