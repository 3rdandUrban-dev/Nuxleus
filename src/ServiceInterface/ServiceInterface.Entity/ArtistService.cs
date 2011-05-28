using Nuxleus.Data;
using Nuxleus.ServiceModel.Operations;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceInterface;

namespace Nuxleus.ServiceInterface
{
    public class ArtistService
        : RestServiceBase<Artist>
    {
        public IDataRepository Repository { get; set; }

        public override object OnGet (Artist request)
        {
            return new ArtistResponse
            {
                ArtistInfo = Repository.GetArtist(request),
            };
        }

        public override object OnPost (Artist request)
        {
            return new ArtistResponse
            {
                ArtistInfo = Repository.CreateArtist(request),
            };
        }

        public override object OnPut (Artist request)
        {
            return new ArtistResponse
            {
                ArtistInfo = Repository.UpdateArtist(request),
            };
        }
    }
}