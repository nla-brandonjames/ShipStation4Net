﻿#region License
/*
 * Copyright 2017 Brandon James
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using Newtonsoft.Json.Linq;
using ShipStation4Net.Clients.Interfaces;
using ShipStation4Net.Domain.Entities;
using ShipStation4Net.Responses;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShipStation4Net.Clients
{
    public class Stores : ClientBase, IGets<Store>, IUpdates<Store>
    {
        public Stores(Configuration configuration) : base(configuration)
        {
            BaseUri = "stores";
        }

        public Task<Store> GetAsync(int id)
        {
            return GetDataAsync<Store>(id);
        }

        /// <summary>
        /// Updates an existing store. This call does not currently support partial updates. The entire resource must be provided in the 
        /// body of the request.
        /// </summary>
        /// <param name="id">
        /// A unique ID generated by ShipStation and assigned to each store.
        /// Example: 12345. </param>
        /// <param name="item">The store data to update.</param>
        /// <returns>The updated store.</returns>
        public Task<Store> UpdateAsync(int id, Store item)
        {
            return PutDataAsync(id, item);
        }

        /// <summary>
        /// Retrieve the list of installed stores on the account.
        /// </summary>
        /// <returns>A list of installed stores on the account.</returns>
        public Task<IList<Store>> GetItemsAsync(bool showInactive = false, int? marketplaceId = null)
        {
            var filter = (showInactive || marketplaceId != null) ? "?" : "";
            if (showInactive)
            {
                filter += "showInactive=true";
            }
            if (showInactive && marketplaceId != null)
            {
                filter += "&";
            }
            if (marketplaceId != null)
            {
                filter += "marketplaceId=" + marketplaceId.Value.ToString();
            }
            return GetDataAsync<IList<Store>>(filter);
		}

		/// <summary>
		/// Retrieves the refresh status of a given store. 
		/// </summary>
		/// <param name="storeId">
		/// Specifies the store whose status will be retrieved.
		/// Example: 12345.</param>
		/// <returns>A response containing details regarding the store refresh status.</returns>
		public Task<StoreRefreshStatusResponse> GetStoreRefreshStatusAsync(int storeId)
        {
            return GetDataAsync<StoreRefreshStatusResponse>($"getrefreshstatus?storeId={storeId}");
		}

		/// <summary>
		/// Initiates store refresh for all stores on account.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RefreshAllStoresAsync()
		{
			var response = await PostDataAsync<JObject, SuccessResponse>("refreshstore", new JObject());

			return response.Success;
		}

		/// <summary>
		/// Initiates a store refresh.
		/// </summary>
		/// <param name="storeId">
		/// Specifies the store which will get refreshed.If the storeId is not specified, a store refresh will be initiated for all 
		/// refreshable stores on that account.
		/// </param>
		/// <param name="refreshDate">
		/// Specifies the starting date for new order imports.If the refreshDate is not specified, ShipStation will use the last recorded refreshDate for that store.
		/// </param>
		/// <returns>A response indicating the success status of the refresh.</returns>
		public async Task<bool> RefreshStoreAsync(int storeId, DateTime refreshDate)
        {
            var refreshStoreRequest = new JObject();
            refreshStoreRequest["storeId"] = storeId;
            refreshStoreRequest["refreshDate"] = refreshDate;

            var response = await PostDataAsync<JObject, SuccessResponse>("refreshstore", refreshStoreRequest).ConfigureAwait(false);

            return response.Success;
        }

        /// <summary>
        /// Lists the marketplaces that can be integrated with ShipStation.
        /// </summary>
        /// <returns></returns>
        public Task<IList<Marketplace>> GetMarketplacesAsync()
        {
            return GetDataAsync<IList<Marketplace>>("marketplaces");
        }

        /// <summary>
        /// Deactivates the specified store.
        /// </summary>
        /// <param name="storeId">ID of the store to deactivate.</param>
        /// <returns>A response indicating whether or not the request was successful.</returns>
        public async Task<bool> DeactivateStoreAsync(int storeId)
        {
            var deactivateStoreRequest = new JObject();
            deactivateStoreRequest["storeId"] = storeId;

            var response = await PostDataAsync<JObject, SuccessResponse>("deactivate", deactivateStoreRequest).ConfigureAwait(false);

            return response.Success;
        }

        /// <summary>
        /// Reactivates the specified store. Note: stores are active by default
        /// </summary>
        /// <param name="storeId">ID of the store to reactivate.</param>
        /// <returns>A response indicating whether or not the request was successful.</returns>
        public async Task<bool> ReactivateStoreAsync(int storeId)
        {
            var deactivateStoreRequest = new JObject();
            deactivateStoreRequest["storeId"] = storeId;

            var response = await PostDataAsync<JObject, SuccessResponse>("reactivate", deactivateStoreRequest).ConfigureAwait(false);

            return response.Success;
        }
    }
}
