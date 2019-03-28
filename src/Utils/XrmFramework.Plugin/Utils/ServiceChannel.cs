// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugins
{
    public class ServiceChannel<T> where T : IService
    {
        private readonly T _service;

        private readonly T _serviceInstance;

        /// <summary>
        /// Crée une nouvelle instance.
        /// </summary>
        public ServiceChannel(LocalContext localContext)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Not an interface");
            }
            _serviceInstance = (T)ServiceManager.GetService(typeof(T), localContext);

            //_service = LoggingProxy<T>.Create(_serviceInstance, localContext);
            _service = _serviceInstance;
        }

        /// <summary>
        /// Implémentation du service.
        /// </summary>
        public T Service
        {
            get
            {
                return _service;
            }
        }

        /// <summary>
        /// Libère les ressources.
        /// </summary>
        public void Dispose()
        {
            if (_serviceInstance != null)
            {
                //if (_serviceInstance is IDisposable && !ServiceManager.IsSingleInstanceService(_serviceInstance.GetType()))
                //{
                //    ((IDisposable)_serviceInstance).Dispose();
                //}
            }
        }
    }
}
