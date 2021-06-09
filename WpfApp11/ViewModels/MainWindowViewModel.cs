using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using RandomDataToolsInterop;
using RandomDataToolsInterop.Models;
using WpfApp11.Context;
using WpfApp11.Extensions;
using WpfApp11.Extensions.Models;
using WpfApp11.Model.DbModels;
using WpfApp11.Models;
using WpfApp11.Views.Windows;

namespace WpfApp11.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<DisplayUser> DisplayUsers { get; } =
            new ObservableCollection<DisplayUser>();

        public ICollectionView DisplayUsersView { get; }

        public DisplayUser SelectedUser { get; set; }

        /// <summary>
        /// Индекс текущей страницы (от 0).
        /// </summary>
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                if (value == _currentPageIndex) return;
                _currentPageIndex = value;
                OnPropertyChanged();
                UpdatePaginationInfo();
                RefreshDisplayUsersView();
            }
        }

        public int PagesCount
        {
            get => _pagesCount;
            set
            {
                if (value == _pagesCount) return;
                _pagesCount = value;
                OnPropertyChanged();
            }
        }

        public int ShownItemsFrom
        {
            get => _shownItemsFrom;
            set
            {
                if (value == _shownItemsFrom) return;
                _shownItemsFrom = value;
                OnPropertyChanged();
            }
        }

        public int ShownItemsTo
        {
            get => _shownItemsTo;
            set
            {
                if (value == _shownItemsTo) return;
                _shownItemsTo = value;
                OnPropertyChanged();
            }
        }

        public int ItemsCount
        {
            get => _itemsCount;
            set
            {
                if (value == _itemsCount) return;
                _itemsCount = value;
                OnPropertyChanged();
            }
        }

        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                if (value == _itemsPerPage) return;
                _itemsPerPage = value;
                OnPropertyChanged();
            }
        }

        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private BoolVisibility _hasUsers = new BoolVisibility(true);
        private int _itemsCount;
        private int _pagesCount;
        private int _currentPageIndex;
        private int _shownItemsFrom;
        private int _shownItemsTo;
        private int _itemsPerPage = 10;

        private void UpdatePaginationInfo()
        {
            PagesCount = (int) Math.Ceiling((double) ItemsCount / ItemsPerPage);
            ShownItemsFrom = ItemsPerPage * CurrentPageIndex + 1;
            var lastItemIndex = DisplayUsers.Count;
            var to = ShownItemsFrom + ItemsPerPage - 1;
            ShownItemsTo = lastItemIndex < to ? lastItemIndex : to;
        }

        public BoolVisibility HasUsers
        {
            get => _hasUsers;
            set
            {
                if (Equals(value, _hasUsers)) return;
                _hasUsers = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            DisplayUsersView = CollectionViewSource.GetDefaultView(DisplayUsers);
            DisplayUsersView.Filter += DisplayUsersViewFilter;
            DisplayUsers.CollectionChanged += DisplayUsersOnCollectionChanged;
            DataLoaded += OnDataLoaded;
            LoadData();
        }

        private bool DisplayUsersViewFilter(object obj)
        {
            UpdatePaginationInfo();
            var user = (DisplayUser) obj;
            var collectionUser = DisplayUsers.First(x => x.User.Id == user.User.Id);
            var userIndex = DisplayUsers.IndexOf(collectionUser);
            return userIndex >= ShownItemsFrom - 1 && userIndex <= ShownItemsTo - 1;
        }

        private void RefreshDisplayUsersView()
        {
            DisplayUsersView.Refresh();
        }

        private void OnDataLoaded(object sender, EventArgs e) => UpdateCollectionInfo();

        private void DisplayUsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateCollectionInfo();

        private void UpdateCollectionInfo()
        {
            ItemsCount = DisplayUsers.Count;
            UpdateHasUsers();
            RefreshDisplayUsersView();
        }

        private void UpdateHasUsers()
        {
            HasUsers.Value = DisplayUsers.Any();
        }

        private async void LoadData()
        {
            using (var db = new AppDbContext())
            {
                await db.Users.ForEachAsync(user =>
                {
                    _synchronizationContext.Send(_ =>
                    {
                        DisplayUsers.Add(new DisplayUser(user));
                    }, null);
                });
            }

            DataLoaded?.Invoke(this, EventArgs.Empty);
        }

        private event EventHandler DataLoaded;

        private static readonly Random Random = new Random();

        public ICommand AddCommand => new Command(async parameter =>
        {
            Person personInfo;
            while (true)
            {
                try
                {
                    personInfo = await Api.GetPersonAsync();
                    break;
                }
                catch (HttpRequestException)
                {
                    await Task.Delay(100);
                }
            }
            
            var user = new User
            {
                FullName = personInfo.FullName,
                Username = personInfo.Login,
                Photo = BitmapExtensions.GetRandomBitmap(240, Random.NextOf(2, 3, 4, 5, 6, 8, 10)).GetBytes()
            };

            using (var db = new AppDbContext())
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            _synchronizationContext.Send(d =>
            {
                DisplayUsers.Add(new DisplayUser(user));
            }, null);
        });

        public ICommand RemoveCommand => new Command(async parameter =>
        {
            if (parameter == null) return;
            var selectedItems = (IEnumerable)parameter;
            var selectedUsers = selectedItems.Cast<DisplayUser>().ToList();
            using (var db = new AppDbContext())
            {
                foreach (var user in selectedUsers)
                {

                    var dbUser = await db.Users.FindAsync(user.User.Id);
                    if (dbUser == null) continue;
                    db.Users.Remove(dbUser);

                    DisplayUsers.Remove(user);
                }

                await db.SaveChangesAsync();
            }
        }, parameter => SelectedUser != null);

        public ICommand ShowImageGeneratorWindowCommand => new Command(property =>
        {
            new RandomImageGeneratorWindow().Show();
        });

        public ICommand FirstPageCommand => new Command(property =>
        {
            CurrentPageIndex = 0;
        }, property => CurrentPageIndex > 0);

        public ICommand PreviousPageCommand => new Command(property =>
        {
            CurrentPageIndex--;
        }, property => CurrentPageIndex > 0);

        public ICommand NextPageCommand => new Command(property =>
        {
            CurrentPageIndex++;
        }, property => CurrentPageIndex < PagesCount - 1);

        public ICommand LastPageCommand => new Command(property =>
        {
            CurrentPageIndex = PagesCount - 1;
        }, property => CurrentPageIndex < PagesCount - 1);
    }
}
