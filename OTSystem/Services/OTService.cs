using System;
using System.Collections.Generic;
using System.Threading;
using EasyModbus;

namespace OTSystem.Services
{
    public class OTService
    {
        private readonly ModbusServer _server;
        private readonly HashSet<int> _processing = new();
        private readonly object _lock = new();

        public OTService()
        {
            _server = new ModbusServer();
        }

        public void Start()
        {
            _server.Listen();

            _server.holdingRegisters[0] = 20; 
            _server.holdingRegisters[1] = 0; 
            _server.holdingRegisters[2] = 0;  

            Console.WriteLine("[OT] Server startad (lager=20, orderId=0, status=0).");

            new Thread(() =>
            {
                while (true)
                {
                    int orderId = _server.holdingRegisters[1];
                    int status = _server.holdingRegisters[2];

                    if (orderId > 0 && status == 1) 
                    {
                        lock (_lock)
                        {
                            if (!_processing.Contains(orderId))
                            {
                                _processing.Add(orderId);
                                Console.WriteLine($"[OT] Ny order upptäckt: {orderId}");

                                new Thread(() => SimulateOrderFlow(orderId))
                                {
                                    IsBackground = true
                                }.Start();

                                _server.holdingRegisters[1] = 0; 
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true }.Start();
        }

        private void SimulateOrderFlow(int orderId)
        {
            short[] statuses = { 1, 2, 3, 4 };
            string[] labels = { "Betald", "Packas", "Skickad", "Klar" };

            for (int i = 0; i < statuses.Length; i++)
            {
                Thread.Sleep(5000);
                _server.holdingRegisters[2] = statuses[i];
                Console.WriteLine($"[OT] Order {orderId} → {labels[i]}");

                if (statuses[i] == 2)
                {
                    _server.holdingRegisters[0] -= 5;
                    Console.WriteLine($"[OT] Lager minskat: {_server.holdingRegisters[0]} kvar");

                    if (_server.holdingRegisters[0] < 5)
                    {
                        Console.WriteLine("[OT] Lågt lager, fyller på...");
                        Thread.Sleep(10000);
                        _server.holdingRegisters[0] += 20;
                        Console.WriteLine($"[OT] Lager påfyllt: {_server.holdingRegisters[0]} st");
                    }
                }
            }

            Console.WriteLine($"[OT] Order {orderId} färdig.");
            lock (_lock) { _processing.Remove(orderId); }
        }
    }
}