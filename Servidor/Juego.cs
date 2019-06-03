﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Servidor
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Juego
    {
        [JsonProperty]
        public Stack<Carta> Mazo { get; set; }

        [JsonProperty]
        public List<Carta> CartasComunes { get; set; }

        [JsonProperty]
        public List<Jugador> Jugadores { get; set; }

        [JsonProperty]
        public int ApuestaAlta { get; set; }

        [JsonProperty]
        public int ApuestaMinima { get; set; }

        [JsonProperty]
        public int Bote { get; set; }

        [JsonProperty]
        public int Ronda { get; set; }

        public Juego()
        {
            LlenarMazo();
            CartasComunes = new List<Carta>();
            Jugadores = new List<Jugador>();
            Ronda = 0;
        }

        void LlenarMazo()
        {
            string[] leyendas = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            List<Carta> cartas = new List<Carta>();

            for (int i = 0; i < 4; i++)
            {
                for (int p = 0; p < 13; p++)
                {
                    cartas.Add(new Carta(leyendas[p], i));
                }
            }

            Barajar(cartas);
            Mazo = new Stack<Carta>(cartas);
        }

        public static void Barajar(List<Carta> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Carta value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

        }

        public void Flop()
        {
            for (int i = 0; i < 3; i++)
            {
                CartasComunes.Add(Mazo.Pop());
            }

            Ronda = 1;
        }

        public void Turn()
        {
            CartasComunes.Add(Mazo.Pop());
            Ronda = 2;
        }

        public void River()
        {
            CartasComunes.Add(Mazo.Pop());
            Ronda = 3;
        }

        public void Repartir()
        {
            foreach (Jugador jugador in Jugadores)
            {
                jugador.Mano[0] = Mazo.Pop();
                jugador.Mano[1] = Mazo.Pop();
            }
        }

    }
}
