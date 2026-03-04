namespace Movies.Api.ApiEndpoints;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Movies
    {
        private const string Base = $"{ApiBase}/movies";

        public const string Create = Base;
        public const string Get = $"{Base}/{{idOrSlug}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        
        // where are rating going to lives?
        // rating are directly related to movies
        
        public const string Rate = $"{Base}/{{id:guid}}/ratings";
        public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
        
    }

    // get all the rating for user

    public static class Ratings
    {
        private const string Base = $"{ApiBase}/ratings";

        public const string GetUserRatings = $"{Base}/me";
    }
}