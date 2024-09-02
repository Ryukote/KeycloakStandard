using KeycloakStandard.Models;

namespace KeycloakStandard
{
    public class Client
    {
        private ClientData _clientData = new ClientData();

        /// <summary>
        /// Client constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public Client(ClientData clientData)
        {
            _clientData = clientData;
        }
    }
}
