using PDRepository.Branches;
using PDRepository.Documents;
using PDRepository.Models;
using PDRepository.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository
{
   /// <summary>
   /// The main entry point for all repository methods.
   /// </summary>
   public class RepositoryClient : IDisposable
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RepositoryClient"/> class.
      /// </summary>
      /// <param name="settings">A RepositorySettings instance.</param>
      protected RepositoryClient(RepositorySettings settings)
      {
         this.BranchClient = new BranchClient(settings);
         this.DocumentClient = new DocumentClient(settings);
         this.ModelClient = new ModelClient(settings);
         this.UserClient = new UserClient(settings);
      }

      /// <summary>
      /// Creates the client object with the specified settings.
      /// </summary>
      /// <param name="settings">A RepositorySettings instance.</param>      
      public static RepositoryClient CreateClient(RepositorySettings settings) 
      {
         return new RepositoryClient(settings);
      }

      /// <summary>
      /// Entry point to Branches
      /// </summary>
      public IBranchClient BranchClient { get; }

      /// <summary>
      /// Entry point to Documents
      /// </summary>
      public IDocumentClient DocumentClient { get; }

      /// <summary>
      /// Entry point to Models
      /// </summary>
      public IModelClient ModelClient { get; }

      /// <summary>
      /// Entry point to Users
      /// </summary>
      public IUserClient UserClient { get; }

      /// <summary>
      /// Disposes the <see cref="RepositoryClient"/> class.
      /// </summary>
      public void Dispose()
      {
         BranchClient?.Dispose();
         DocumentClient?.Dispose(); 
         ModelClient?.Dispose(); 
         UserClient?.Dispose();
      }

      /// <summary>
      /// Returns the version information associated with this assembly.
      /// </summary>
      public string Version {
         get 
         {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersion.FileVersion;
         }
      }
   }
}
