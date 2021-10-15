using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
    public class State : IState
    {
        public bool IsAuthenticated { get; set; }

        public IEnumerable<Repo> Repositories { get; set; } = Enumerable.Empty<Repo>();

        public bool RepositoriesStale { get; set; }

        public event Action OnChange;
        
        // Don't auto-trigger to allow for batch updates of state
        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
