using System;
using System.Collections.Generic;
using System.Linq;

public class Assessment : IAssessment
{
    /// <summary>
    /// Returns the score with the highest value
    /// </summary>
    public Score? WithMax(IEnumerable<Score> scores)
    {
        Score scoreObject = new Score();
        var maxScore = scores.Max().Value;
        if(maxScore>0)
          scoreObject.Value = maxScore;

        return scoreObject;
    }

    /// <summary>
    /// Returns the average value of the collection. For an empty collection it returns null
    /// </summary>
    public double? GetAverageOrDefault(IEnumerable<int> items)
    {
         double maxValue=items.Average();

        Console.WriteLine(maxValue);

        return maxValue;
    }


    /// <summary>
    /// Appends the suffix value to the text if the text has value. If not, it returns empty.
    /// </summary>
    public string WithSuffix(string text, string suffixValue)
    {
        string results = string.Empty;
         results = text.Trim();
        if (!string.IsNullOrEmpty(suffixValue))
        {
            results = results.Replace("\n", suffixValue + "\n");
        }
        return results;
    }

    /// <summary>
    /// It fetches all the data from the source.
    /// </summary>
    /// <param name="source">The source data provider returns items by page. NextPageToken is the page token of the next page. If there are no more items to return,
    /// nextPageToken will be empty. Passing a null or empty string to the provider will return the first page of the data.
    /// If no value is specified for nextPageToken, the provider will return the first page.
    /// </param>
    /// <returns></returns>
    public IEnumerable<Score> GetAllScoresFrom(IDataProvider<Score> source)
    {
        IEnumerable<Score> results = new List<Score>();

        var token = "1";

        do
        {
            if (token != null)
            {
                var sou = source.GetData(token);
                if (sou != null)
                {
                    results = sou.Items;
                    token = sou.NextPageToken;
                }
            }
            else
            {
                var sou = source.GetData("1");
                if (sou != null)
                {
                    results = sou.Items;
                    token = sou.NextPageToken;
                }

            }

        } while (token!=null);

        return results;
        
    }

    /// <summary>
    /// Returns child's name prefixed with all its parents' names separated by the specified separator.Example : Parent/Child
    /// </summary>
    public string GetFullName(IHierarchy child, string separator = null)
    {
        
        child = child.Parent;
        separator ??= "/";
        var hierarchy=child.Parent.Name+separator+child.Name;

        return hierarchy;
        
    }

    /// <summary>
    /// Refactor: Returns the value that is closest to the average value of the specified numbers.
    /// </summary>
    public int? ClosestToAverageOrDefault(IEnumerable<int> numbers)
    {

        var count=numbers.Count();
        var sum=numbers.Sum(x => x);
        var average=sum/count;
        return numbers.OrderBy(x =>Math.Abs(average-x)).First();
        
    }

    /// <summary>
    /// Returns date ranges that have similar bookings on each day in the range.
    /// Read the example carefully.
    /// Example : [{Project:HR, Date: 01/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 01/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 02/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 02/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 03/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 03/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 04/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 04/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:ECom, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:ECom, Date: 06/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 06/02/2020 , Allocation: 15}
    ///            {Project:ECom, Date: 07/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 07/02/2020 , Allocation: 15}]    
    /// Returns : 
    ///          [
    ///            { From:01/02/2020 , To:02/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:10 }]  },
    ///            { From:03/02/2020 , To:04/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 }]  },
    ///            { From:05/02/2020 , To:05/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 },{ Project:ECom , Allocation:15 }]  },
    ///            { From:06/02/2020 , To:07/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:ECom , Allocation:10 }]  }
    ///          ]
    /// </summary>
    /// 

    // I did not create main static class just I hard coded set the data into list of objects
    public IEnumerable<BookingGrouping> Group(IEnumerable<Booking> dates)
    {
        List<Booking> bookings = GetBookingData();

        var bookingList = bookings.OrderBy(x => x.Date).GroupBy(x => x.Date.ToString("yyyy-MM"))
           .Select(g => new BookingGrouping
           {
               From = g.Min(x => x.Date),
               To = g.Max(x => x.Date),
               Items = g.Select(x => new BookingGroupingItem()
               { Project = x.Project, Allocation = x.Allocation }).ToList()
           }).ToList();



        return bookingList;


    }

   
    /// <summary>
    /// Merges the specified collections so that the n-th element of the second list should appear after the n-th element of the first collection. 
    /// Example : first : 1,3,5 second : 2,4,6 -> result : 1,2,3,4,5,6
    /// </summary>
    public IEnumerable<int> Merge(IEnumerable<int> first, IEnumerable<int> second)
    {
        SortedDictionary<int, Boolean> mapping = new SortedDictionary<int, Boolean>();
        var firstList= first.Count();
        var secondList= second.Count();
        var firstArray=first.ToArray();
        var secondArray=second.ToArray();
        for (int i = 0; i < firstList; i++)
        {
            mapping.Add(firstArray[i], true);
        }
        for (int i = 0; i < secondList; i++)
        {
            mapping.Add(secondArray[i], true);
        }

      IEnumerable<int> list= (IEnumerable<int>)mapping.Values.ToList();
        return list;

    }

    private static List<Booking> GetBookingData()
    {
        return new List<Booking>()
        {
            new Booking()
            {
                Project ="HR",
                Allocation=14,
                Date=new DateTime(2022, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified)
            },
            new Booking()
            {
                Project ="CRM",
                Allocation=10,
                 Date=new DateTime(2022, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified)
            },
            new Booking()
            {
                Project ="ECom",
                Allocation=14,
               Date=new DateTime(2022, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified)
            },
            new Booking()
            {
                Project ="HR",
                Allocation=14,
                Date=new DateTime(2022, 6, 02, 0, 0, 0, 0, DateTimeKind.Unspecified)
            },
            new Booking()
            {
                Project ="HR",
                Allocation=19,
                Date=new DateTime(2022, 6, 02, 0, 0, 0, 0, DateTimeKind.Unspecified)
            },

        };
    }


 }

   


