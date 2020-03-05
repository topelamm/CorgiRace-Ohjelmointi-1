using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class CorgiRace : PhysicsGame
{
    Vector nopeusYlos = new Vector(0, 200);
    Vector nopeusAlas = new Vector(0, -200);
    Vector nopeusVas = new Vector(-200, 0);
    Vector nopeusOik = new Vector(200, 0);

    //PhysicsObject koiro;
    //PhysicsObject maila2;

    //PhysicsObject vasenReuna;
    //PhysicsObject oikeaReuna;

   //IntMeter pelaajan1Pisteet;
    //IntMeter pelaajan2Pisteet;

    public override void Begin()
    {
        // Kirjoita ohjelmakoodisi tähän

        SetWindowSize(1024, 768);
        TileMap kentta = TileMap.FromLevelAsset("kentta");
        kentta.SetTileMethod('x', LuoLattia);
        kentta.SetTileMethod('t', LuoPallo);
        kentta.SetTileMethod('p', LuoPelaaja);
        kentta.Execute(160, 160);

        Level.CreateBorders();
        Camera.ZoomToLevel();


        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        //LuoKentta();
        //AsetaOhjaimet();
        //LisaaLaskurit();
        //AloitaPeli();

    }

    public void LuoPelaaja(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject koiro = new PhysicsObject(leveys, korkeus);
        koiro.Position = paikka;
        koiro.Shape = Shape.Circle;
        koiro.Image = LoadImage("koiro");
        Add(koiro);

        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikuta koiraa oikealle", koiro, 500.0, 0.0);
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikuta koiraa vasemmalle", koiro, -500.0, 0.0);
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, "Liikuta koiraa yläs", koiro, 0.0, 500.0);
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, "Liikuta koiraa alas", koiro, 0.0, -500.0);

    }

    public void Liikuta(PhysicsObject liikuteltavaOlio, double x, double y)
    {
        liikuteltavaOlio.Move(new Vector(x, y));
    }

    public void LuoPallo(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = new PhysicsObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Image = LoadImage("pallo");
        taso.MakeStatic();
        Add(taso);

    }

    public void LuoLattia(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject lattia = new PhysicsObject(leveys, korkeus);
        lattia.Position = paikka;
        lattia.Image = LoadImage("lattia");
        lattia.MakeStatic();
        Add(lattia);
    }

   // void LuoKentta()
    //{
       

      //  koiro = LuoKoira(Level.Left + 20.0, -200.0);
        //maila2 = LuoKoira(Level.Left + 20.0, 200.0);

        //vasenReuna = Level.CreateLeftBorder();
        //vasenReuna.Restitution = 1.0;
        //vasenReuna.IsVisible = false;

        //oikeaReuna = Level.CreateRightBorder();
        //oikeaReuna.Restitution = 1.0;
        //oikeaReuna.IsVisible = false;

        //PhysicsObject alaReuna = Level.CreateBottomBorder();
        //alaReuna.Restitution = 1.0;
        //alaReuna.IsVisible = false;

        //PhysicsObject ylaReuna = Level.CreateTopBorder();
        //ylaReuna.Restitution = 1.0;
        //ylaReuna.IsVisible = false;

        //Level.Background.Color = Color.Green;


        //Camera.ZoomToLevel();
  //  }
   
   // void AloitaPeli()
  //  {
   //     Vector impulssi = new Vector(-250.0, 50.0);
        
  //  }

  //  PhysicsObject LuoKoira(double x, double y)
  //  {
  //      PhysicsObject Koiro = PhysicsObject.CreateStaticObject(200, 200);
  //      Koiro.Shape = Shape.Circle;
  //      Koiro.X = x;
  //      Koiro.Y = y;
  //      Koiro.Restitution = 1.0;
  //      Koiro.Image = LoadImage("Koiro.png");



 //       Add(Koiro);
 //       return Koiro;
  //  }
 //   void AsetaOhjaimet()
 //   {
 //       Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa ylös", koiro, nopeusYlos);
 //       Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, koiro, Vector.Zero);
 //       Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa alas", koiro, nopeusAlas);
 //       Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, koiro, Vector.Zero);
 //       Keyboard.Listen(Key.A, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa vasemmalle", koiro, nopeusVas);
 //       Keyboard.Listen(Key.A, ButtonState.Released, AsetaNopeus, null, koiro, Vector.Zero);
 //       Keyboard.Listen(Key.D, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa oikealle", koiro, nopeusOik);
 //       Keyboard.Listen(Key.D, ButtonState.Released, AsetaNopeus, null, koiro, Vector.Zero);

 //       Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa ylös", maila2, nopeusYlos);
 //       Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, maila2, Vector.Zero);
 //       Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa alas", maila2, nopeusAlas);
 //       Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, maila2, Vector.Zero);
 //       Keyboard.Listen(Key.Left, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa vasemmalle", maila2, nopeusVas);
 //       Keyboard.Listen(Key.Left, ButtonState.Released, AsetaNopeus, null, maila2, Vector.Zero);
 //       Keyboard.Listen(Key.Right, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa oikealle", maila2, nopeusOik);
 //       Keyboard.Listen(Key.Right, ButtonState.Released, AsetaNopeus, null, maila2, Vector.Zero);

//        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
//        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
//    }

   // void AsetaNopeus(PhysicsObject maila, Vector nopeus)
  //  {
   //     if ((nopeus.Y < 0) && (maila.Bottom < Level.Bottom))
   //     {
  //          maila.Velocity = Vector.Zero;
  //          return;
  //      }
 //       if ((nopeus.Y > 0) && (maila.Top > Level.Top))
 //       {
 //           maila.Velocity = Vector.Zero;
 //           return;
 //       }
 //       maila.Velocity = nopeus;


   // }
   // void LisaaLaskurit()
 //   {
 //       pelaajan1Pisteet = LuoPisteLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
 //       pelaajan2Pisteet = LuoPisteLaskuri(Screen.Right - 100.0, Screen.Top - 100.0);
 //   }

 //   IntMeter LuoPisteLaskuri(double x, double y)
 //   {
 //       IntMeter laskuri = new IntMeter(0);
 //       laskuri.MaxValue = 10;
 //
 //       Label naytto = new Label();
 //       naytto.BindTo(laskuri);
 //       naytto.X = x;
 //       naytto.Y = y;
 //       naytto.TextColor = Color.White;
 //       naytto.BorderColor = Level.Background.Color;
 //       naytto.Color = Level.Background.Color;
 //       Add(naytto);

   //     return laskuri;
   // }
}
