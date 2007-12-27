using System;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class AspNetAwsConfiguration : ConfigurationSection {


    public static AspNetAwsConfiguration GetConfig() {
      return (AspNetAwsConfiguration)ConfigurationManager.GetSection("Xameleon.WebApp/aws");
    }

    [ConfigurationProperty("awsKeyCollection", IsRequired = true)]
    public AwsKeyCollection AwsKeyCollection {
      get {
        return this["awsKeyCollection"] as AwsKeyCollection;
      }
    }

    [ConfigurationProperty("awsS3", IsRequired = true)]
    public AwsS3Collection AwsS3 {
      get {
        return this["awsS3"] as AwsS3Collection;
      }
    }
  }
}
