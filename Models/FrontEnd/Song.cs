namespace Models.FrontEnd
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public Song() { }

        public Song(int id, string title, string subtitle)
        {
            Id = id;
            Title = title;
            Subtitle = subtitle;
        }
    }
}
