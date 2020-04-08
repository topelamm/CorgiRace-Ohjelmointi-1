using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author Toni Lamminaho
/// @version 1.0
/// <summary>
/// Koirat keräävät kilpaa palloja
/// </summary>
public class CorgiRace : PhysicsGame
{
    Vector nopeusYlos = new Vector(0, 200);
    Vector nopeusAlas = new Vector(0, -200);
    Vector nopeusOik = new Vector(200, 0);
    Vector nopeusVas = new Vector(-200, 0);

    Vector aloitus1 = new Vector(-800, 400);
    Vector aloitus2 = new Vector(-800, -400);

    PhysicsObject koiro1;
    PhysicsObject koiro2;

    IntMeter pelaajan1Pisteet;
    IntMeter pelaajan2Pisteet;
    IntMeter laskuri;

    const int maksimiTormaukset = 11;
    const int laskurinPaikka = 50;
    const int laskurinKorkeus = 50;
    const int listanPituus = 10;
    const int listalleMinPisteet = 0;
    const int koironKoko = 150;
    public override void Begin()
    {
        SetWindowSize(1280, 720);
        Level.Background.Image = LoadImage("Tausta");

        TileMap kentta = TileMap.FromLevelAsset("kentta");
        kentta.SetTileMethod('x', LuoLattia);
        kentta.SetTileMethod('t', LuoPallo);
        kentta.Execute(50, 50);

        koiro1 = LuoPelaaja1(koironKoko);
        AddCollisionHandler(koiro1, Tormaa);
        void Tormaa(PhysicsObject koiro, PhysicsObject kohde)
        {
            if (kohde.Tag.Equals("pallo"))
            {
                pelaajan1Pisteet.AddValue(+1);
            }
        }

        koiro2 = LuoPelaaja2(koironKoko);
        AddCollisionHandler(koiro2, Tormaa2);
        void Tormaa2(PhysicsObject koiro, PhysicsObject kohde)
        {
            if (kohde.Tag.Equals("pallo"))
            {
                pelaajan2Pisteet.AddValue(+1);
            }
        }

        Level.CreateBorders();
        Camera.ZoomToLevel();

        LisaaLaskurit();
        AsetaOhjaimet();
    }


     /// <summary>
     /// Luo pelaajan
     /// 
     /// </summary>
     /// <param name="paikka">pelaajan aloituspaikka</param>
     /// <param name="leveys">pelaajan hahmon leveys</param>
     /// <param name="korkeus">pelaajan hahmon korkeus</param>
     /// <param name="kuva">pelaajan kuva</param>
     /// <returns>Palauttaa pelaajan perusteet</returns>
   private PhysicsObject LuoPelaaja(Vector paikka, double leveys, double korkeus, Image kuva)
    {
        PhysicsObject koiro = new PhysicsObject(leveys, korkeus);
        koiro.Position = paikka;
        koiro.Shape = Shape.Circle;
        koiro.Image = kuva;
        koiro.CanRotate = false;
        koiro.IgnoresExplosions = true;
        koiro.Tag = "pelaaja";
        AddCollisionHandler(koiro, Tormaa);
        Add(koiro);
        return koiro;

        
        void Tormaa(PhysicsObject koiro, PhysicsObject kohde)
        {
            if (kohde.Tag.Equals("pelaaja"))
            {
                Explosion rajahdys = new Explosion(kohde.Width);
                rajahdys.Position = kohde.Position;
                Add(rajahdys);
            }
            if (kohde.Tag.Equals("pallo"))
            {
                laskuri.AddValue(+1);
                Explosion rajahdys = new Explosion(kohde.Width);
                rajahdys.Position = kohde.Position;
                Add(rajahdys);
                kohde.Destroy();
            }
        }
    }


    /// <summary>
    /// Luo ensimmäisen pelaajan
    /// </summary>
    /// <param name="koko"></param>
    /// <returns>Palauttaa pelaajan, jonka kokoa voi muokata</returns
    private PhysicsObject LuoPelaaja1(double koko)
    {
        return LuoPelaaja(aloitus1, koko, koko, LoadImage("koiro"));
    }


    /// <summary>
    /// Luo toisen pelaajan
    /// </summary>
    /// <param name="koko"></param>
    /// <returns>
    /// Palauttaa pelaajan, jonka kokoa voi muokata</returns>
    private PhysicsObject LuoPelaaja2(double koko)
    {
        return LuoPelaaja(aloitus2, koko, koko, LoadImage("VihuKoiro"));
    }


    /// <summary>
    /// Luo kerättäviä palloja pelikentälle
    /// </summary>
    /// <param name="paikka"> pallon paikka kentällä</param>
    /// <param name="leveys"> pallon leveys</param>
    /// <param name="korkeus"> pallon korkeus</param>
    public void LuoPallo(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject pallo = new PhysicsObject(leveys, korkeus);
        pallo.Position = paikka;
        pallo.Image = LoadImage("pallo");
        pallo.MakeStatic();
        pallo.Tag = "pallo";
        Add(pallo);    
    }
    
    
    /// <summary>
    /// Luo pelikentälle lattiapaloja
    /// </summary>
    /// <param name="paikka"> lattiapalan paikka</param>
    /// <param name="leveys"> lattiapalan leveys</param>
    /// <param name="korkeus"> lattiapalan korkeus</param>
    public void LuoLattia(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject lattia = new PhysicsObject(leveys, korkeus);
        lattia.Position = paikka;
        lattia.Image = LoadImage("lattia");
        lattia.Tag = "lattia";
        lattia.MakeStatic();
        Add(lattia);
    }


    /// <summary>
    /// Luo pelaajien ohjaimet sekä "ohje" ja "lopetus" näppäimet
    /// </summary>
   void AsetaOhjaimet()
    {
         Keyboard.Listen(Key.W, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa ylös", koiro1, nopeusYlos);
         Keyboard.Listen(Key.W, ButtonState.Released, AsetaNopeus, null, koiro1, Vector.Zero);
         Keyboard.Listen(Key.S, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa alas", koiro1, nopeusAlas);
         Keyboard.Listen(Key.S, ButtonState.Released, AsetaNopeus, null, koiro1, Vector.Zero);
         Keyboard.Listen(Key.A, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa vasemmalle", koiro1, nopeusVas);
         Keyboard.Listen(Key.A, ButtonState.Released, AsetaNopeus, null, koiro1, Vector.Zero);
         Keyboard.Listen(Key.D, ButtonState.Down, AsetaNopeus, "Pelaaja 1: Liikuta koiraa oikealle", koiro1, nopeusOik);
         Keyboard.Listen(Key.D, ButtonState.Released, AsetaNopeus, null, koiro1, Vector.Zero);

         Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa ylös", koiro2, nopeusYlos);
         Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, koiro2, Vector.Zero);
         Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa alas", koiro2, nopeusAlas);
         Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, koiro2, Vector.Zero);
         Keyboard.Listen(Key.Left, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa vasemmalle", koiro2, nopeusVas);
         Keyboard.Listen(Key.Left, ButtonState.Released, AsetaNopeus, null, koiro2, Vector.Zero);
         Keyboard.Listen(Key.Right, ButtonState.Down, AsetaNopeus, "Pelaaja 2: Liikuta koiraa oikealle", koiro2, nopeusOik);
         Keyboard.Listen(Key.Right, ButtonState.Released, AsetaNopeus, null, koiro2, Vector.Zero);
     
         Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");       
         Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");   
    }


    /// <summary>
    /// Asettaa pelaajahahmolle nopeuden 
    /// </summary>
    /// <param name="koiro"> hahmo, jolle nopeus asetetaan</param>
    /// <param name="nopeus"> nopeus vektorina</param>
    void AsetaNopeus(PhysicsObject koiro, Vector nopeus)
    {
        if ((nopeus.Y < 0) && (koiro.Bottom < Level.Bottom))
        {
            koiro.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.Y > 0) && (koiro.Top > Level.Top))
        {
            koiro.Velocity = Vector.Zero;
            return;
        }

        koiro.Velocity = nopeus;
    }


    /// <summary>
    /// Luo pistelaskurit kentälle pelaajille sekä laskurin montako palloa on yhteensä kerätty kentältä
    /// </summary>
    void LisaaLaskurit()
    {
     pelaajan1Pisteet = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - laskurinKorkeus);
     pelaajan2Pisteet = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - 13*laskurinKorkeus);
     laskuri = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - 7* laskurinKorkeus);
    }


    /// <summary>
    /// Määrittää pistelaskurin. Pistelaskurin maksimiarvo 10, kun se saavutetaan peli päättyy
    /// </summary>
    /// <param name="x"> pistelaskurin x-koordinaatti pelikentällä</param>
    /// <param name="y"> pistelaskurin y-koordinaatti pelikentällä</param>
    /// <returns></returns>
    IntMeter LuoPisteLaskuri(double x, double y)
    {
     IntMeter pistelaskuri = new IntMeter(0);
     pistelaskuri.MaxValue = maksimiTormaukset;
     pistelaskuri.UpperLimit += PeliLoppui;
        
     Label naytto = new Label();
     naytto.BindTo(pistelaskuri);
     naytto.X = x;
     naytto.Y = y;
     naytto.TextColor = Color.Black;
     naytto.BorderColor = Color.Gray;
     naytto.Color = Color.Gray;
     Add(naytto);

     return pistelaskuri;
    }


    /// <summary>
    /// Lopettaa pelin ja jos pelaaja pääsee top-listalle, antaa kirjoittaa pelaajan nimen
    /// Näyttää toplistan ja antaa ilmoituksen mahdollisuudesta aloittaa uuden pelin
    /// </summary>
    void PeliLoppui()
    {

        ScoreList topLista = new ScoreList(listanPituus, false, listalleMinPisteet);
        topLista = DataStorage.TryLoad<ScoreList>(topLista, "pisteet.xml");

        HighScoreWindow topIkkuna = new HighScoreWindow(
        "Parhaat pisteet",
        "Onneksi olkoon, pääsit listalle pisteillä %p",
        topLista,
        Suurempi()
        );
    Add(topIkkuna);

    topIkkuna.Closed += delegate (Window ikkuna)
            {
                DataStorage.Save<ScoreList>(topLista, "pisteet.xml");
            };

    topIkkuna.Closed += delegate (Window ikkuna)
            {
                MessageDisplay.Add("Aloita uusi peli");
            };
    }
    

    /// <summary>
    /// Kertoo pelin lopettavalle aliohjelmalle kumpi pelaajista voittaa. Tasapelissä kumpikaan ei voita.
    /// </summary>
    /// <returns> Palauttaa voittavan pelaajan pisteet kokonaislukuna</returns>
    public int Suurempi()
    {
        if (pelaajan1Pisteet.Value > pelaajan2Pisteet.Value) return pelaajan1Pisteet;
        return pelaajan2Pisteet; 
    }


}