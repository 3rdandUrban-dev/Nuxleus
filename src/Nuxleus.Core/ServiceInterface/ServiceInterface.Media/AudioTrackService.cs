//using Nuxleus.Data;
//using Nuxleus.ServiceModel.Operations;
//using Nuxleus.ServiceModel.Types;
//using ServiceStack.ServiceInterface;
//using Nuxleus.ServiceModel.Operations.Media;
//
//namespace Nuxleus.ServiceInterface
//{
//    public class AudioTrackService
//        : RestServiceBase<AudioTrack>
//    {
//        public IDataRepository Repository { get; set; }
//
//        public override object OnGet (AudioTrack request)
//        {
//            return new AudioTrackResponse
//            {
//                AudioTrackInfo = Repository.GetOrCreateAudioTrack(request),
//            };
//        }
//
//        public override object OnPost (AudioTrack request)
//        {
//            return new AudioTrackResponse
//            {
//                AudioTrackInfo = Repository.GetOrCreateAudioTrack(request), 
//            };
//        }
//
//        public override object OnPut (AudioTrack request)
//        {
//            return new AudioTrackResponse
//            {
//                AudioTrackInfo = Repository.GetOrCreateAudioTrack(request),
//            };
//        }
//    }
//}