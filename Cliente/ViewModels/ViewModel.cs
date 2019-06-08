﻿using Cliente.Models;
using Cliente.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cliente
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Juego Juego { get; set; }
        public Jugador Jugador { get; set; }
        public string CartaMano1 { get; set; }
        public string CartaMano2 { get; set; }
        public string CartaFlop1 { get; set; }
        public string CartaFlop2 { get; set; }
        public string CartaFlop3 { get; set; }
        public string CartaTurn { get; set; }
        public string CartaRiver { get; set; }
        public bool Jugando { get; set; }

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ViewModel()
        {
            // Constructor
        }

        public List<Jugador> ObtenerJugadores()
        {
            return Juego.Jugadores;
        }

        public void ActualizarInfoJugador()
        {
            foreach (Jugador jugador in Juego.Jugadores)
            {
                if (jugador.NombreUsuario.Equals(ClienteTCP.Name()))
                {
                    Jugador = jugador;
                    OnPropertyChange("Jugador");
                }
            }
        }

        public void ObtenerMano(string nombre)
        {
            CartaMano1 = "/Resources/Images/Cards/" + Jugador.Mano[0].TipoPalo + Jugador.Mano[0].Leyenda + ".png";
            CartaMano2 = "/Resources/Images/Cards/" + Jugador.Mano[1].TipoPalo + Jugador.Mano[1].Leyenda + ".png";
            OnPropertyChange("CartaMano1");
            OnPropertyChange("CartaMano2");
        }

        public void ObtenerFlop()
        {
            CartaFlop1 = "/Resources/Images/Cards/" + Juego.CartasComunes[0].TipoPalo + Juego.CartasComunes[0].Leyenda + ".png";
            CartaFlop2 = "/Resources/Images/Cards/" + Juego.CartasComunes[1].TipoPalo + Juego.CartasComunes[1].Leyenda + ".png";
            CartaFlop3 = "/Resources/Images/Cards/" + Juego.CartasComunes[2].TipoPalo + Juego.CartasComunes[2].Leyenda + ".png";
            OnPropertyChange("CartaFlop1");
            OnPropertyChange("CartaFlop2");
            OnPropertyChange("CartaFlop3");
        }

        public void ObtenerTurn()
        {
            CartaTurn = "/Resources/Images/Cards/" + Juego.CartasComunes[3].TipoPalo + Juego.CartasComunes[3].Leyenda + ".png";
            OnPropertyChange("CartaTurn");
        }

        public void ObtenerRiver()
        {
            CartaRiver = "/Resources/Images/Cards/" + Juego.CartasComunes[4].TipoPalo + Juego.CartasComunes[4].Leyenda + ".png";
            OnPropertyChange("CartaRiver");
        }

        public void FlopCall()
        {
            if (Jugador.Role.Equals(Jugador.APUESTA_ALTA))
            {
                return;
            }
            else
            {
                if (Jugador.Role.Equals(Jugador.APUESTA_BAJA))
                {
                    Jugador.ApuestaActual += (Juego.ApuestaAlta - Juego.ApuestaMinima);
                    Jugador.CantFichas -= (Juego.ApuestaAlta - Juego.ApuestaMinima);
                    Juego.Bote += (Juego.ApuestaAlta - Juego.ApuestaMinima);
                }
                else
                {
                    Jugador.ApuestaActual = Juego.ApuestaAlta;
                    Jugador.CantFichas -= Jugador.ApuestaActual;
                    Juego.Bote += Jugador.ApuestaActual;
                }
            }
        }

        public void RegularCall()
        {
            int max = ObtenerApuestaMax();
            int diferencia = 0;

            if (max == Jugador.ApuestaActual)
            {
                Jugador.ApuestaActual += 100;
                Jugador.CantFichas -= 100;
                Juego.Bote += 100;
            }
            else
            {
                if (max > Jugador.ApuestaActual)
                {
                    diferencia = (max - Jugador.ApuestaActual);
                    Jugador.ApuestaActual += diferencia;
                    Jugador.CantFichas = Jugador.CantFichas - diferencia;
                    Juego.Bote += diferencia;
                }
                else
                {
                    return;
                }
            }
        }

        public void Raise(int cantApuesta)
        {
            Jugador.ApuestaActual += cantApuesta;
            Jugador.CantFichas -= cantApuesta;
            Juego.Bote += cantApuesta;
        }

        public int ObtenerApuestaMax()
        {
            int max = 0;

            foreach (Jugador jugador in Juego.Jugadores)
            {
                if (jugador.ApuestaActual > max)
                {
                    max = jugador.ApuestaActual;
                }
            }

            return max;
        }

        public void ActualizarTurno()
        {
            if (Juego.Turno == Jugador.NumJugador)
            {
                Jugando = true;
                OnPropertyChange("Jugando");
            }
            else
            {
                Jugando = false;
                OnPropertyChange("Jugando");
            }
        }

        public void Actualizar()
        {
            Console.WriteLine("Actualizando juego...");
            Juego = JsonConvert.DeserializeObject<Juego>(ClienteTCP.Read());
            OnPropertyChange("Juego");

            switch (Juego.Ronda)
            {
                case 1:
                    ActualizarInfoJugador();
                    ObtenerMano(ClienteTCP.Name());
                    break;

                case 2:
                    ObtenerFlop();
                    break;

                case 3:
                    ObtenerTurn();
                    break;

                case 4:
                    ObtenerRiver();
                    break;

                default:
                    break;
            }
        }

    }
}
