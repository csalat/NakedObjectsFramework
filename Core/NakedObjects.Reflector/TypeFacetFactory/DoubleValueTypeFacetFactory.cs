// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Reflect.TypeFacetFactory {
    public class DoubleValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
        public DoubleValueTypeFacetFactory(int numericOrder) : base(numericOrder) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (DoubleValueSemanticsProvider.IsAdaptedType(type)) {
                var spec = reflector.LoadSpecification<IObjectSpecImmutable> (DoubleValueSemanticsProvider.AdaptedType);
                AddValueFacets(new DoubleValueSemanticsProvider(spec, specification), specification);
            }
        }
    }
}