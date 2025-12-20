using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Factories;
using Services.Interface;
using UI.Base;
using UnityEngine;
using VContainer;

namespace Services
{
    public class ScreenService : IScreenService
    {
        [Inject] private readonly ScreensFactory _screensFactory;
        
        private readonly Dictionary<Type, View> _screensByType = new();
        private readonly LinkedList<View> _screens = new();
        
        public View CurrentScreen { get; private set; }

        public async UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View
        {
            return await OpenAsyncInternal<TScreen, object>(default, false);
        }

        public async UniTask<TScreen> OpenAsync<TScreen, TPayload>(TPayload payload) where TScreen : View
        {
            return await OpenAsyncInternal<TScreen, TPayload>(payload, true);
        }

        public TScreen OpenSync<TScreen>() where TScreen : View
        {
            return OpenSyncInternal<TScreen, object>(default, false);
        }

        public TScreen OpenSync<TScreen, TPayload>(TPayload payload) where TScreen : View
        {
            return OpenSyncInternal<TScreen, TPayload>(payload, true);
        }

        public void ClearCollections()
        {
            _screensByType.Clear();
            _screens.Clear();
        }

        public void CloseScreen<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                _screens.Remove(screen);
                screen.Close();
                screen.gameObject.SetActive(false);
                
                if (_screens.Count > 0)
                    CurrentScreen = _screens.Last.Value;
            }
        }

        public void Close()
        {
            var screen = _screens.Last.Value;

            if (screen == null) 
                return;
            
            _screens.RemoveLast();
            screen.Close();
            screen.gameObject.SetActive(false);
            
            if (_screens.Count > 0)
            {
                CurrentScreen = screen;
            }
        }

        private async UniTask<TScreen> OpenAsyncInternal<TScreen, TPayload>(TPayload payload, bool hasPayload)
            where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var cachedScreen))
            {
                var typedScreen = (TScreen)cachedScreen;

                if (hasPayload)
                {
                    typedScreen.ApplyPayload(payload);
                }

                return OpenScreen(typedScreen, false);
            }

            var newScreen = await _screensFactory.CreateAsync<TScreen>();

            if (hasPayload)
            {
                newScreen.ApplyPayload(payload);
            }

            return OpenScreen(newScreen, true);
        }

        private TScreen OpenSyncInternal<TScreen, TPayload>(TPayload payload, bool hasPayload) where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var cachedScreen))
            {
                var typedScreen = (TScreen)cachedScreen;

                if (hasPayload)
                {
                    typedScreen.ApplyPayload(payload);
                }

                return OpenScreen(typedScreen, false);
            }

            var newScreen = _screensFactory.CreateSync<TScreen>();

            if (hasPayload)
            {
                newScreen.ApplyPayload(payload);
            }

            return OpenScreen(newScreen, true);
        }

        private TScreen OpenScreen<TScreen>(TScreen screen, bool addToCache) where TScreen : View
        {
            screen.Open();

            if (addToCache)
            {
                _screensByType.Add(typeof(TScreen), screen);
            }

            _screens.AddLast(screen);
            CurrentScreen = screen;

            return screen;
        }
    }
}
