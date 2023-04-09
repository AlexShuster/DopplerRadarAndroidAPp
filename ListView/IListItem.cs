namespace DopplerRadarAndroidApp.ListView
{
    public interface IListItem
    {
        ListItemType GetListItemType();

        string Text { get; set; }
    }
}