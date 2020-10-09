using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using U4DosRandomizer;

namespace U4Randomizer.Visualizer
{
    public partial class Form1 : Form
    {
        private List<Thing> things;
        private bool closing = false;
        private byte[,] worldTiles;
        private WorldMapAbstract world;
        private Image map;
        public Form1()
        {
            InitializeComponent();
            int seed = 93857937;
            seed = 4481843;
            Random rand = new Random(seed);


            world = new WorldMapUnchanged();
            world.Load("C:\\Program Files (x86)\\GOG Galaxy\\Games\\Ultima 4", rand.Next(), new Random(rand.Next()), new Random(rand.Next()));
            worldTiles = world.ToArray();
            var sixImage = world.ToImage();

            //https://codeblog.vurdalakov.net/2019/06/imagesharp-convert-image-to-system-drawing-bitmap-and-back.html
            using (var memoryStream = new MemoryStream())
            {
                var imageEncoder = sixImage.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                sixImage.Save(memoryStream, imageEncoder);

                memoryStream.Seek(0, SeekOrigin.Begin);

                map = new System.Drawing.Bitmap(memoryStream);
            }

            things = new List<Thing>();
            //for (int i = 0; i < 8; i++)
            //{
            //    things.Add(new Thing() { X = Thing.Wrap((256 / 2) + i + rand.Next(0, 3)), Y = Thing.Wrap((256 / 2) + i + rand.Next(0, 3)) });
            //    //things.Add(new Thing() { X = Thing.Wrap(rand.Next(0,256)), Y = Thing.Wrap(rand.Next(0, 256)) });
            //}
            PlaceThings(rand);

            //Graphics g;

            //g = this.CreateGraphics();

            //DrawEllipse();

            //Thread thread1 = new Thread(DoWork);
            //thread1.Start();
        }

        private void PlaceThings(Random random)
        {
            var numCandidates = 10;

            var possibleLocations = world.GetAllMatchingTiles(WorldMapGenerateMap.IsWalkableGround);
            //possibleLocations.RemoveAll(c => excludeLocations.Contains(c));

            var randomIdx = random.Next(0, possibleLocations.Count);
            var original = possibleLocations[randomIdx];

            things.Add(new Thing() { X = original.X, Y = original.Y });

            // Towns
            for (int i = 0; i < 8-1; i++)
            {
                ITile bestCandidate = null;
                var bestDistance = 0;
                for (int sample = 0; sample < numCandidates; sample++)
                {
                    randomIdx = random.Next(0, possibleLocations.Count);
                    var selection = possibleLocations[randomIdx];
                    var distance = world.DistanceSquared(FindClosestThing(selection), selection);

                    if( distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestCandidate = selection;
                    }
                }

                things.Add(new Thing() { X = bestCandidate.X, Y = bestCandidate.Y });
            }
        }

        private ICoordinate FindClosestThing(ITile selection)
        {
            Thing closest = null;
            var closestDistance = int.MaxValue;
            for(int i = 0; i < things.Count(); i++)
            {
                var distance = world.DistanceSquared(selection, world.GetCoordinate(things[i].X, things[i].Y));
                if(distance > 0 && distance < closestDistance)
                {
                    closest = things[i];
                    closestDistance = distance;
                }
            }

            return world.GetCoordinate(closest.X, closest.Y);
        }

        private void DoWork(object obj)
        {
            Thread.Sleep(3000);
            while (!closing)
            {
                Console.WriteLine("Working thread...");
                //things[0].X = Thing.Wrap(things[0].X + 1);

                for(int i = 0; i < things.Count; i++)
                {
                    things[i].ForceX = 0;
                    things[i].ForceY = 0;
                }

                for (int i = 0; i < things.Count; i++)
                {
                    var origin = things[i];

                    for (int j = i+1; j < things.Count; j++)
                    {
                        var destination = things[j];
                        double deltaX = destination.X - origin.X;
                        if (Math.Abs(deltaX) > 256 / 2)
                        {
                            if (deltaX >= 0)
                            {
                                deltaX = deltaX - 256;
                            }
                            else
                            {
                                deltaX = 256 + deltaX;
                            }
                        }

                        double deltaY = destination.Y - origin.Y;
                        if (Math.Abs(deltaY) > 256 / 2)
                        {
                            if (deltaY >= 0)
                            {
                                deltaY = deltaY - 256;
                            }
                            else
                            {
                                deltaY = 256 + deltaY;
                            }
                        }

                        var constant = -66.6;
                        origin.ForceX += deltaX == 0 ? 10 : constant * (10.0 / (deltaX * deltaX)) * (deltaX > 0 ? 1.0 : -1.0);
                        origin.ForceY += deltaY == 0 ? 10 : constant * (10.0 / (deltaY * deltaY)) * (deltaY > 0 ? 1.0 : -1.0);
                        destination.ForceX += deltaX == 0 ? -10 : constant * (10.0 / (deltaX * deltaX)) * (deltaX > 0 ? -1.0 : 1.0);
                        destination.ForceY += deltaY == 0 ? -10 : constant * (10.0 / (deltaY * deltaY)) * (deltaY > 0 ? -1.0 : 1.0);
                    }
                }

                

                for (int i = 0; i < things.Count; i++)
                {
                    Invalidate(new Rectangle(things[i].X + 1, things[i].Y + 1, 4, 4));
                    things[i].X = Thing.Wrap(things[i].X + things[i].ForceX);
                    things[i].Y = Thing.Wrap(things[i].Y + things[i].ForceY);
                    Invalidate(new Rectangle(things[i].X + 1, things[i].Y + 1, 4, 4));
                }

                //Console.WriteLine($"X: {things[5].X} Y: {things[5].Y}");

                Thread.Sleep(100);
            }
        }

        public static int DistanceSquared(Thing destination, Thing origin)
        {
            var deltaX = Math.Abs(destination.X - origin.X);
            if (deltaX > 256 / 2)
            {
                deltaX = 256 - deltaX;
            }
            var deltaY = Math.Abs(destination.Y - origin.Y);
            if (deltaY > 256 / 2)
            {
                deltaY = 256 - deltaY;
            }
            var distanceSquared = (deltaX * deltaX + deltaY * deltaY);

            return distanceSquared;
        }

        private void DrawFrame(PaintEventArgs e)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Cyan);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.DrawRectangle(myPen, new Rectangle(0, 0, 258, 258));
            //formGraphics.DrawEllipse(myPen, new Rectangle(0, 0, 1, 1));

            myPen.Dispose();
            formGraphics.Dispose();
        }

        private void DrawEllipse(PaintEventArgs e)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.DrawEllipse(myPen, new Rectangle(0, 0, 1, 1));
            
            myPen.Dispose();
            formGraphics.Dispose();
        }
        private void DrawThings(PaintEventArgs e)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            foreach(var thing in things)
            {
                formGraphics.DrawEllipse(myPen, new Rectangle(thing.X+1, thing.Y+1, 3, 3));
            }

            myPen.Dispose();
            formGraphics.Dispose();
        }

        private void DrawMap(PaintEventArgs e)
        {
            e.Graphics.DrawImage(map, new Point(1, 1));
            //System.Drawing.Pen grass = new System.Drawing.Pen(System.Drawing.Color.Green);
            //System.Drawing.Pen mountain = new System.Drawing.Pen(System.Drawing.Color.Gray);
            //System.Drawing.Pen ocean = new System.Drawing.Pen(System.Drawing.Color.Blue);
            //System.Drawing.Graphics formGraphics;
            //formGraphics = e.Graphics;

            //for(int x = 0; x < 256; x++)
            //{
            //    for (int y = 0; y < 256; y++)
            //    {
            //        Pen pen = mountain;
            //        var tile = worldTiles[x, y];
            //        if ((tile >= TileInfo.Swamp && tile <= TileInfo.Hills) || (tile >= TileInfo.Dungeon_Entrance && tile <= TileInfo.Village))
            //        {
            //            pen = grass;
            //        }
            //        else if (tile == TileInfo.Deep_Water || tile == TileInfo.Medium_Water || tile == TileInfo.Shallow_Water)
            //        {
            //            pen = ocean;
            //        }

            //        formGraphics.DrawEllipse(pen, new Rectangle(x + 1, y + 1, 1, 1));
            //    }
            //}

            //grass.Dispose();
            //mountain.Dispose();
            //ocean.Dispose();
            ////formGraphics.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // If there is an image and it has a location, 
            // paint it when the Form is repainted.
            base.OnPaint(e);
            DrawFrame(e);
            DrawMap(e);
            DrawThings(e);
            //if (this.picture != null && this.pictureLocation != Point.Empty)
            //{
            //    e.Graphics.DrawImage(this.picture, this.pictureLocation);
            //}
        }

        protected override void OnClosed(EventArgs e)
        {
            closing = true;
        }


    }
}
