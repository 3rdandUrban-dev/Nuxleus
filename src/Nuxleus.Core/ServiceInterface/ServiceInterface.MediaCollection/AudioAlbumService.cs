using Nuxleus.Data;
using Nuxleus.ServiceModel.Operations;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using Nuxleus.ServiceModel.Operations.MediaCollections;

namespace Nuxleus.ServiceInterface
{
    public class AudioAlbumService
        : RestServiceBase<AudioAlbum>
    {
        public IDataRepository Repository { get; set; }

        public override object OnGet(AudioAlbum request)
        {
            return new AudioAlbumResponse
            {

            };
        }

        public override object OnPost(AudioAlbum request)
        {
            return new AudioAlbumResponse
            {

            };
        }

        public override object OnPut(AudioAlbum request)
        {
            return new AudioAlbumResponse
            {

            };
        }

        public override object OnDelete(AudioAlbum request)
        {
            return new AudioAlbumResponse
            {

            };
        }
    }

    public class AudioAlbumResponse : IHasResponseStatus
    {

        public AudioAlbumInfo AudioAlbumInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}