using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsExtensions.Tiles;

namespace App3
{
    public class TileHelper
    {
        static async Task GenerateImageToFile(string fileName, FrameworkElement uiContent)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await CaptureToStreamAsync(uiContent, stream, BitmapEncoder.PngEncoderId);
                }
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        static async Task<RenderTargetBitmap> CaptureToStreamAsync(FrameworkElement uielement, IRandomAccessStream stream, Guid encoderId)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uielement);
            IBuffer pixels = await renderTargetBitmap.GetPixelsAsync();
            double logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (UInt32)(renderTargetBitmap.PixelWidth), (UInt32)(renderTargetBitmap.PixelHeight), logicalDpi, logicalDpi, pixels.ToArray());
            await encoder.FlushAsync();
            return renderTargetBitmap;
        }

        static void SetLiveTileToSingleImage(string wideImageFileName, string mediumImageFileName)
        {
            // Construct the tile content as a string
            string content = $@"
                               <tile>
                                   <visual> 
                                       <binding template='TileSquareImage'>
                                          <image id='1' src='ms-appdata:///local/{mediumImageFileName}' />
                                       </binding> 
                                        <binding template='TileWideImage' branding='none'>
                                          <image id='1' src='ms-appdata:///local/{wideImageFileName}' />
                                       </binding>
 
                                   </visual>
                               </tile>";

            // Load the string into an XmlDocument
            XmlDocument doc = new XmlDocument();
           // TileContent tc=new TileContent();
            //tc.Visual.TileMedium.
            doc.LoadXml(content);

            // Then create the tile notification
            var notification = new TileNotification(doc);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public static async Task ConvertControlsToTiles(FrameworkElement wideTile, FrameworkElement mediumTile)
        {
            await GenerateImageToFile("wideTile.png", wideTile);
            await GenerateImageToFile("mediumTile.png", mediumTile);
            SetLiveTileToSingleImage("wideTile.png", "mediumTile.png");
        }
    }

    
}
