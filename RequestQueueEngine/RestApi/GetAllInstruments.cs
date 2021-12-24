using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGApi.Common;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        public async Task GetAllInstrumentsAsync(string id = "0")
        {
            var response = await IGRestApiClient.browse(id);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                var test = response.Response;

                if (test != null)
                {
                    if (test.nodes != null)
                    {
                        foreach (var node in test.nodes)
                        {
                            if (node.id != null)
                            {
                                //BrowseNodes.Add(node);
                                Log.WriteLine("Browse Node found NAME: " + node.name);
                                Log.WriteLine("Browse Node found ID: " + node.id);

                                await GetAllInstrumentsAsync(node.id);
                            }
                        }
                    }
                    else if (test.markets != null)
                    {
                        foreach (var market in test.markets)
                        {
                            if (market.epic != null)
                            {
                                //BrowseNodes.Add(node);
                                Log.WriteLine("Browse Market found EPIC: " + market.epic);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.WriteLine("response.StatusCode: " + response.StatusCode.ToString());
            }
        }
    }
}