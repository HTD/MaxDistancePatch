using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;

namespace MaxDistancePatch {
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window {

        /// <summary>
        /// Spinner sequence values.
        /// </summary>
        private readonly static string[] SpinnerText = new[] { "...", "....", ".....", "......" };

        /// <summary>
        /// Current index of the spinner sequence.
        /// </summary>
        private int SpinnerIndex;

        /// <summary>
        /// A list containing patches to apply.
        /// It's created from enumeration which evaluates lazy.
        /// </summary>
        private readonly List<MaxDistance.Patch> PatchList = new List<MaxDistance.Patch>();

        /// <summary>
        /// Creates the window from XAML.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// Visually indicates a start of a long operation.
        /// </summary>
        private void ProcessingStart() {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            Spinner.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Visually indicates a progress of a long operation.
        /// </summary>
        /// <param name="complete">Progress value from 0 to 1 where 1 means complete. Default -1 for indeterminate.</param>
        /// <returns>Awaitable task.</returns>
        private async Task ProcessingUpdateAsync(double complete = -1) {
            if (complete >= 0) {
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                TaskbarItemInfo.ProgressValue = complete;
            }
            Spinner.Text = SpinnerText[SpinnerIndex++];
            await Dispatcher.Yield();
            if (++SpinnerIndex >= SpinnerText.Length) SpinnerIndex = 0;
        }

        /// <summary>
        /// Visually indicates an end of a long operation.
        /// </summary>
        private void ProcessingEnd() {
            Spinner.Visibility = Visibility.Collapsed;
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
        }

        /// <summary>
        /// Checks if the file is run in correct directory.
        /// </summary>
        private void CheckDirectory() {
            if (!MaxDistance.IsValidPath) {
                Close();
                MessageBox.Show(Properties.Resources.InvalidDirectory, Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        /// <summary>
        /// Asynchronously searches for limited visibility nodes.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        private async Task SearchForNodes() {
            Header.Text = Properties.Resources.SearchingForLimitedNodes;
            ProcessingStart();
            await ProcessingUpdateAsync();
            CheckDirectory();
            foreach (var patch in MaxDistance.NodePatches) {
                var item = patch.Path;
                FileList.Items.Add(item);
                FileList.ScrollIntoView(item);
                PatchList.Add(patch);
                await ProcessingUpdateAsync();
            }
            ProcessingEnd();
            if (PatchList.Count > 0) {
                Header.Text = string.Format(Properties.Resources.ReadyNodesFound, FileList.Items.Count);
                Patch.IsEnabled = true;
            }
            else Header.Text = Properties.Resources.AlreadyPatched;
        }

        /// <summary>
        /// Handles main window startup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            await Dispatcher.Yield();
            await SearchForNodes();
        }

        /// <summary>
        /// Handles patch button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Patch_Click(object sender, RoutedEventArgs e) {
            MaxDistance.CreateBackupDir();
            ProcessingStart();
            Header.Text = Properties.Resources.ApplyingPatch;
            int i = 0, n = PatchList.Count;
            try {
                foreach (var patch in PatchList) {
                    MaxDistance.ApplyPatch(patch);
                    await ProcessingUpdateAsync(++i / (double)n);
                    FileList.Items.Remove(patch.Path);
                }
            }
            catch {
                MessageBox.Show(Properties.Resources.CannotWrite, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ProcessingEnd();
            Header.Text = Properties.Resources.AllDone;
            Patch.IsEnabled = false;
        }

    }

}