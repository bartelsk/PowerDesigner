// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
   /// <summary>
   /// This class represents the settings used to connect with the repository.
   /// </summary>
   public class RepositorySettings
   {
      /// <summary>
      /// A repository user.
      /// </summary>
      public string User { get; set; }

      /// <summary>
      /// The password of the repository user.
      /// </summary>
      public string Password { get; set; }
   }
}
