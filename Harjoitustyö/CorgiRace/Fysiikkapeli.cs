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
    const int listanPituus = 10;
    const int listalleMinPisteet = 0;
    const int koironKoko = 150;
    const int lattianKoko = 50;
    const int aitojenMaara = 10;
    const int pisteitäPallosta = 1;
    const double sijaintiMin = 500.0;
    const double sijaintiMax = 550.0;


    public override void Begin()
    {
        SetWindowSize(1280, 720);
        Level.Background.Image = LoadImage("Tausta");

        TileMap kentta = TileMap.FromLevelAsset("kentta");
        kentta.SetTileMethod('x', LuoObjekti, "lattia");
        kentta.SetTileMethod('t', LuoObjekti, "pallo");
        kentta.Execute(lattianKoko, lattianKoko);

        koiro1 = LuoPelaaja1(koironKoko);
        AddCollisionHandler(koiro1, Tormaa);
        void Tormaa(PhysicsObject koiro, PhysicsObject kohde)
        {
            if (kohde.Tag.Equals("pallo"))
            {
                lisaaPisteet(pelaajan1Pisteet);
            }
        }

        koiro2 = LuoPelaaja2(koironKoko);
        AddCollisionHandler(koiro2, Tormaa2);
        void Tormaa2(PhysicsObject koiro, PhysicsObject kohde)
        {
            if (kohde.Tag.Equals("pallo"))
            {
                lisaaPisteet(pelaajan2Pisteet);
            }
        }

        Level.CreateBorders();
        Camera.ZoomToLevel();

        LisaaLaskurit();
        AsetaOhjaimet();
    }


    /// <summary>
    /// Lisää pelaajien laskuriin pisteitä
    /// </summary>
    /// <param name="pelaaja">mihin laskuriin lisätään</param>
    public void lisaaPisteet(IntMeter pelaaja)
    {
        pelaaja.AddValue(+pisteitäPallosta);
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
    }


    /// <summary>
    /// Käsittelee törmäyksen. Pelaajan osuessa toiseen pelaajan törmäys synnyttää 
    /// satunnaisesti koirien ympärille lisää aitaa,
    /// jotka poistuvat osa kerrallaan ajastinmen mukaan.
    /// Pelaajan osuessa palloon kerättyjen pallojen yhteislaskuri kasvaa yhdellä
    /// ja pallo katoaa räjähtäen.
    /// </summary>
    /// <param name="koiro"> Törmäävä olio </param>
    /// <param name="kohde"> Olio, johon törmätään </param>
    public void Tormaa(PhysicsObject koiro, PhysicsObject kohde)
    {
        List<PhysicsObject> lattia =  new List<PhysicsObject>();

        if (kohde.Tag.Equals("pelaaja"))
        {
            Explosion rajahdys = new Explosion(kohde.Width);
            rajahdys.Position = kohde.Position;
            for (int i = 0; i < aitojenMaara; i++)
            {
                Vector sijainti = RandomGen.NextVector(sijaintiMin, sijaintiMax) + koiro.Position;
                PhysicsObject aita = new PhysicsObject(lattianKoko,lattianKoko);
                aita.Position = sijainti;
                aita.Image = LoadImage("lattia");
                aita.MakeStatic();
                lattia.Add(aita);
                Add(aita);
            }
            
            Add(rajahdys);

            Timer poistoAjastin = new Timer();
            poistoAjastin.Interval = 1;
            poistoAjastin.Timeout += delegate ()
            {
                PoistaAita(lattia);
            };
            poistoAjastin.Start();

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


    /// <summary>
    /// Luo ensimmäisen pelaajan
    /// </summary>
    /// <param name="koko"> pelaajan koko</param>
    /// <returns>Palauttaa pelaajan, jonka kokoa voi muokata</returns
    private PhysicsObject LuoPelaaja1(double koko)
    {
        return LuoPelaaja(aloitus1, koko, koko, LoadImage("koiro"));
    }
  
  
    /// <summary>
    /// Luo toisen pelaajan
    /// </summary>
    /// <param name="koko">pelaajan koko</param>
    /// <returns>
    /// Palauttaa pelaajan, jonka kokoa voi muokata</returns>
    private PhysicsObject LuoPelaaja2(double koko)
    {
        return LuoPelaaja(aloitus2, koko, koko, LoadImage("VihuKoiro"));
    }


    /// <summary>
    /// aliohjelma, joka kertoo ajastimelle missä järjestyksessä koirien törmäyksessä
    /// syntyneitä aitapaloja poistetaan
    /// </summary>
    /// <param name="lattia"></param>
    public void PoistaAita(List<PhysicsObject> lattia)
    {
        if (lattia.Count == 0) return;
        lattia[0].Destroy();
        lattia.RemoveAt(0);
    }


    /// <summary>
    /// Luo objekteja pelikentälle pelikentälle
    /// </summary>
    /// <param name="paikka"> objektin paikka kentällä</param>
    /// <param name="leveys"> objentin leveys</param>
    /// <param name="korkeus"> objektin korkeus</param>
    /// <param name="objekti"></param>
    public void LuoObjekti(Vector paikka, double leveys, double korkeus, string objekti)
    {
        PhysicsObject osa = new PhysicsObject(leveys, korkeus);
        osa.Position = paikka;
        osa.Image = LoadImage(objekti);
        osa.MakeStatic();
        osa.Tag = objekti;
        Add(osa);
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
        pelaajan1Pisteet = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - laskurinPaikka);
        pelaajan2Pisteet = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - 13 * laskurinPaikka);
        laskuri = LuoPisteLaskuri(Screen.Left + laskurinPaikka, Screen.Top - 7 * laskurinPaikka);
    }


    /// <summary>
    /// Määrittää pistelaskurin. Kun pistelaskurin maksimi saavutetaan, peli päättyy
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
    /// Kertoo pelin lopettavalle aliohjelmalle kumpi pelaajista voittaa. 
    /// </summary>
    /// <returns> Palauttaa voittavan pelaajan pisteet kokonaislukuna</returns>
    public int Suurempi()
    {
        if (pelaajan1Pisteet.Value > pelaajan2Pisteet.Value) return pelaajan1Pisteet;
        return pelaajan2Pisteet;
    }


}