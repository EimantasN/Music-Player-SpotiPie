using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Result
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("annotation_count")]
        public long? AnnotationCount { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("full_title")]
        public string FullTitle { get; set; }

        [JsonProperty("header_image_thumbnail_url")]
        public Uri HeaderImageThumbnailUrl { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("instrumental")]
        public bool? Instrumental { get; set; }

        [JsonProperty("lyrics_owner_id")]
        public long? LyricsOwnerId { get; set; }

        [JsonProperty("lyrics_state", NullValueHandling = NullValueHandling.Ignore)]
        public string LyricsState { get; set; }

        [JsonProperty("lyrics_updated_at", NullValueHandling = NullValueHandling.Ignore)]
        public long? LyricsUpdatedAt { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("pyongs_count")]
        public long? PyongsCount { get; set; }

        [JsonProperty("song_art_image_thumbnail_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri SongArtImageThumbnailUrl { get; set; }

        [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
        public Stats Stats { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("title_with_featured", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleWithFeatured { get; set; }

        [JsonProperty("updated_by_human_at", NullValueHandling = NullValueHandling.Ignore)]
        public long? UpdatedByHumanAt { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("primary_artist", NullValueHandling = NullValueHandling.Ignore)]
        public Artist PrimaryArtist { get; set; }

        [JsonProperty("about_me_summary", NullValueHandling = NullValueHandling.Ignore)]
        public string AboutMeSummary { get; set; }

        [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
        public Avatar Avatar { get; set; }

        [JsonProperty("human_readable_role_for_display")]
        public object HumanReadableRoleForDisplay { get; set; }

        [JsonProperty("iq", NullValueHandling = NullValueHandling.Ignore)]
        public long? Iq { get; set; }

        [JsonProperty("is_meme_verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMemeVerified { get; set; }

        [JsonProperty("is_verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVerified { get; set; }

        [JsonProperty("login", NullValueHandling = NullValueHandling.Ignore)]
        public string Login { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("role_for_display")]
        public object RoleForDisplay { get; set; }

        [JsonProperty("current_user_metadata", NullValueHandling = NullValueHandling.Ignore)]
        public CurrentUserMetadata CurrentUserMetadata { get; set; }

        [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ImageUrl { get; set; }

        [JsonProperty("index_character", NullValueHandling = NullValueHandling.Ignore)]
        public string IndexCharacter { get; set; }

        [JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug { get; set; }

        [JsonProperty("cover_art_thumbnail_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri CoverArtThumbnailUrl { get; set; }

        [JsonProperty("cover_art_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri CoverArtUrl { get; set; }

        [JsonProperty("name_with_artist", NullValueHandling = NullValueHandling.Ignore)]
        public string NameWithArtist { get; set; }

        [JsonProperty("release_date_components")]
        public ReleaseDateComponents ReleaseDateComponents { get; set; }

        [JsonProperty("artist", NullValueHandling = NullValueHandling.Ignore)]
        public Artist Artist { get; set; }

        [JsonProperty("article_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ArticleUrl { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("dfp_kv", NullValueHandling = NullValueHandling.Ignore)]
        public List<DfpKv> DfpKv { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? Duration { get; set; }

        [JsonProperty("poster_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public PosterAttributes PosterAttributes { get; set; }

        [JsonProperty("poster_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri PosterUrl { get; set; }

        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public string Provider { get; set; }

        [JsonProperty("provider_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderId { get; set; }

        [JsonProperty("provider_params", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProviderParam> ProviderParams { get; set; }

        [JsonProperty("published_at", NullValueHandling = NullValueHandling.Ignore)]
        public long? PublishedAt { get; set; }

        [JsonProperty("short_title", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortTitle { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultType { get; set; }

        [JsonProperty("video_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public PosterAttributes VideoAttributes { get; set; }

        [JsonProperty("view_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? ViewCount { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public Author Author { get; set; }

        [JsonProperty("sponsorship", NullValueHandling = NullValueHandling.Ignore)]
        public Sponsorship Sponsorship { get; set; }

        [JsonProperty("article_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleType { get; set; }

        [JsonProperty("dek", NullValueHandling = NullValueHandling.Ignore)]
        public string Dek { get; set; }

        [JsonProperty("draft", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Draft { get; set; }

        [JsonProperty("for_homepage", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ForHomepage { get; set; }

        [JsonProperty("for_mobile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ForMobile { get; set; }

        [JsonProperty("preview_image", NullValueHandling = NullValueHandling.Ignore)]
        public Uri PreviewImage { get; set; }

        [JsonProperty("sponsor_image")]
        public dynamic SponsorImage { get; set; }

        [JsonProperty("sponsor_image_style", NullValueHandling = NullValueHandling.Ignore)]
        public string SponsorImageStyle { get; set; }

        [JsonProperty("sponsor_link", NullValueHandling = NullValueHandling.Ignore)]
        public string SponsorLink { get; set; }

        [JsonProperty("sponsor_phrase", NullValueHandling = NullValueHandling.Ignore)]
        public string SponsorPhrase { get; set; }

        [JsonProperty("sponsored", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Sponsored { get; set; }

        [JsonProperty("votes_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? VotesTotal { get; set; }
    }
}
