using Carter.Response;
using Interfaces.Requests;
using System;

namespace CarterModule.Modules
{
    public class HomeModule : CarterModuleProxy
    {
        public HomeModule()
        {
            // Unautorize
            // this.RequiresAuthorization();

            DTORoute<TestRequest>(async (req, res) => {
                // session 확인
                var model = await req.BindOrDefault<TestRequest>().ConfigureAwait(false);
                Console.WriteLine(model.Message);

                var response = new TestRequest.Response { Message = "Ok" };

                await res.AsJson(response).ConfigureAwait(false);
            });
        }
    }
}
