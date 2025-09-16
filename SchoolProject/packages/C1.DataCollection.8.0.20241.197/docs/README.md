## About

The ComponentOne DataCollection library includes a series of cross-platform observable collections with features like sorting, grouping and filtering and data virtualization techniques like cursor and pagination.

Commonly Used Types:

- C1.DataCollection.C1DataCollection
- C1.DataCollection.C1VirtualDataCollection
- C1.DataCollection.C1CursorDataCollection
- C1.DataCollection.C1PagedDataCollection

#### In-memory Data Collections

[C1DataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1DataCollection%601.html) is the main class used to sort, filter and group any in memory collection like lists, arrays or observable collection

```C#
var collection = new ObservableCollection<Item>();
var dc = new C1DataCollection<Item>(collection);
await dc.SortAsync("Property1", "Property2");
```

All the data-collections implement [INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged?view=net-5.0), when a change happens in the underlying ObservableCollection, the change will be reflected and notified immediatly in the data-collection.

Similarly, the C1DataCollection can be grouped and filtered

```C#
var collection = new ObservableCollection<Item>();
var dc = new C1DataCollection<Item>(collection);
await dc.GroupAsync("Property1");
```
```C#
var collection = new ObservableCollection<Item>();
var dc = new C1DataCollection<Item>(collection);
await dc.FilterAsync("Property1", FilterOperation.Contains, "X");
```

#### Virtualizing Data Collections

[C1VirtualDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1VirtualDataCollection%601.html) and [C1CursorDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1CursorDataCollection%602.html) are two abstract collection that can be used to implement collection whose items are pulled on demand from an external source, typically a network call.

```C#
public class YouTubeCollectionView : C1CursorDataCollection<YouTubeVideo>
{
    protected override async Task<Tuple<string, IReadOnlyList<YouTubeVideo>>> GetPageAsync(int startingIndex, string pageToken, int? count = null, IReadOnlyList<SortDescription> sortDescriptions = null, FilterExpression filterExpresssion = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var client = new HttpClient();
        var response = await client.GetAsync(youtubeUrl, cancellationToken);
        var videos = new List<YouTubeVideo>();
        var serializer = new DataContractJsonSerializer(typeof(YouTubeSearchResult));
        var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as YouTubeSearchResult;
        foreach (var item in result.Items)
        {
            videos.Add(new YouTubeVideo(item));
        }
        return new Tuple<string, IReadOnlyList<YouTubeVideo>>(result.NextPageToken, videos);
    }
}
```
#### Composition Data Collections

There are a series of data collections that can be used to compose other collections

- [C1PagedDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1PagedDataCollection%601.html)
- [C1SequenceDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1SequenceDataCollection%601.html)
- [C1SelectDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1SelectDataCollection%602.html)
- [C1SelectManyDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1SelectManyDataCollection%602.html)
- [C1CacheDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1CacheDataCollection%601.html)
- [C1EditableDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1EditableDataCollection%601.html)
- [C1SpanDataCollection](https://developer.mescius.com/componentone/docs/services/online-datacollection/C1.DataCollection~C1.DataCollection.C1SpanDataCollection%601.html)

#### Extensions

Built-in extensions ease the creation of data collections from know sources like EntityFrameworkCore, Ado.Net and BindingList.

- [C1.DataCollection.EntityFrameworkCore](https://www.nuget.org/packages/C1.DataCollection.EntityFrameworkCore/)
- [C1.DataCollection.AdoNet](https://www.nuget.org/packages/C1.DataCollection.AdoNet/)
- [C1.DataCollection.BindingList](https://www.nuget.org/packages/C1.DataCollection.BindingList/)

#### Adapters

Built-in adapters are used to binding data-collections to native controls.

- [C1.Win.DataCollection](https://www.nuget.org/packages/C1.Win.DataCollection/)
- [C1.WPF.DataCollection](https://www.nuget.org/packages/C1.WPF.DataCollection/)
- [C1.Android.DataCollection](https://www.nuget.org/packages/C1.Android.DataCollection/)
- [C1.iOS.DataCollection](https://www.nuget.org/packages/C1.iOS.DataCollection/)
- [C1.Mac.DataCollection](https://www.nuget.org/packages/C1.Mac.DataCollection/)
- [C1.UWP.DataCollection](https://www.nuget.org/packages/C1.UWP.DataCollection/)

#### Serialization

[C1.DataCollection.Serialization](https://www.nuget.org/packages/C1.DataCollection.Serialization/) includes [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) converters to serialize and deserialize filter, sort and notification classes to Json.

## Resources

- [Samples](https://github.com/GrapeCity/ComponentOne-Service-Components-Samples/tree/master/DataCollection)
- [Documentation](https://developer.mescius.com/componentone/docs/services/online-datacollection/overview.html)
- [Licensing](https://developer.mescius.com/componentone/licensing)