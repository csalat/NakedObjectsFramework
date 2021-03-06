﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.Linq;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect;
using NakedObjects.Reflect.Component;
using NakedObjects.Unity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace NakedObjects.SystemTest.Reflect {
    [TestClass]
    public class ReflectorTest {
        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected static void RegisterFacetFactories(IUnityContainer container) {
            var factoryTypes = FacetFactories.StandardFacetFactories();
            for (int i = 0; i < factoryTypes.Length; i++) {
                RegisterFacetFactory(factoryTypes[i], container, i);
            }
        }

        private static int RegisterFacetFactory(Type factory, IUnityContainer container, int order) {
            container.RegisterType(typeof (IFacetFactory), factory, factory.Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order));
            return order;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            //ReflectorConfiguration.NoValidate = true;
            //RegisterFacetFactories(container);

            //container.RegisterType<IMenuFactory, NullMenuFactory>();
            //container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
            //    new ContainerControlledLifetimeManager(), new InjectionConstructor());
            //container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            //container.RegisterType<IReflector, Reflector>();
            //container.RegisterType<IMetamodel, Metamodel>();
            //container.RegisterType<IMetamodelBuilder, Metamodel>();

            StandardUnityConfig.RegisterStandardFacetFactories(container);
            StandardUnityConfig.RegisterCoreContainerControlledTypes(container);
            StandardUnityConfig.RegisterCorePerTransactionTypes<HierarchicalLifetimeManager>(container);
        }


        private static string[] ModelNamespaces
        {
            get
            {
                return new string[] { "AdventureWorksModel" };
            }
        }

        private static Type[] Types
        {
            get
            {
                return new[] {
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof (OrderStatus),
                    typeof (QuickOrderForm),
                    typeof (ProductProductPhoto),
                    typeof (ProductModelProductDescriptionCulture)
                };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                    typeof (PersonRepository),
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository),
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions),
                    typeof (SpecialOfferContributedActions),
                    typeof (ServiceWithNoVisibleActions)
                };
            }
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            var customerMenu = factory.NewMenu<CustomerRepository>(false);
            CustomerRepository.Menu(customerMenu);
            var salesMenu = factory.NewMenu<SalesRepository>(false);
            SalesRepository.Menu(salesMenu);
            return new[] {
                customerMenu,
                factory.NewMenu<OrderRepository>(true),
                factory.NewMenu<ProductRepository>(true),
                factory.NewMenu<EmployeeRepository>(true),
                salesMenu,
                factory.NewMenu<SpecialOfferRepository>(true),
                factory.NewMenu<PersonRepository>(true),
                factory.NewMenu<VendorRepository>(true),
                factory.NewMenu<PurchaseOrderRepository>(true),
                factory.NewMenu<WorkOrderRepository>(true),
                factory.NewMenu<ServiceWithNoVisibleActions>(true, "Empty")
            };
        }

        [TestMethod]
        public void ReflectAdventureworks() {
            // load adventurework

            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            Assert.AreEqual(142, reflector.AllObjectSpecImmutables.Length);
            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            Assert.IsTrue(interval.Milliseconds < 1000);
            Console.WriteLine(interval.Milliseconds);
        }


        #region Nested type: NullMenuFactory

        public class NullMenuFactory : IMenuFactory {
            #region IMenuFactory Members

            public IMenu NewMenu<T>(bool addAllActions, string name = null) {
                throw new NotImplementedException();
            }

            public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
                throw new NotImplementedException();
            }

            #endregion

            public IMenu NewMenu(string name) {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}