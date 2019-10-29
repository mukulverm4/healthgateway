//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.Medication.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using HealthGateway.Database.Models;

    /// <summary>
    /// Static class that maps between DrugProcucts (Database) and Medication (SimpleJSON) objects.
    /// </summary>
    public static class SimpleModelMapper
    {
        /// <summary>
        /// Maps a list of drug products to medication objects.
        /// </summary>
        /// <param name="drugProducts">The DrugProducts to map.</param>
        /// <returns>A list of mapped medications.</returns>
        public static List<Medication> ToMedicationList(List<DrugProduct> drugProducts)
        {
            Contract.Requires(drugProducts != null);

            List<Medication> medicationList = new List<Medication>(drugProducts.Count);
            foreach (DrugProduct product in drugProducts)
            {
                medicationList.Add(ToMedication(product));
            }

            return medicationList;
        }

        /// <summary>
        /// Maps a drug product to a medication object.
        /// </summary>
        /// <param name="drugProduct">The DrugProduct to map.</param>
        /// <returns>A mapped medication object.</returns>
        public static Medication ToMedication(DrugProduct drugProduct)
        {
            Contract.Requires(drugProduct != null);

            return new Medication()
            {
                DIN = drugProduct.DrugIdentificationNumber,
                BrandName = drugProduct.BrandName,

                // TODO: Map the rest of the information
            };
        }
    }
}