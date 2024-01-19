using App.Model;
using App.Web.Dto;
using App.Web.Repository.Interfaces;
using App.Web.Services.Interfaces;
using App.Web.ViewModel.AppClient;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly IAppClientRepo _clientRepo;
        private readonly IAppClientService _clientService;

        public ClientController(IAppClientRepo clientRepo, IAppClientService clientService)
        {
            _clientRepo = clientRepo;
            _clientService = clientService;
        }

        public IActionResult Index(AppClientReportVm vm)
        {
            vm.SoftwareVersions = _clientRepo.GetQueryable().Select(e => e.CurrentVersion).Distinct().ToList();

            if (vm.IsSearch)
            {
                var clients = _clientRepo.GetQueryable();
                if (vm.CurrentVersion != null)
                {
                    clients = clients.Where(e => e.CurrentVersion.Equals(vm.CurrentVersion));
                }

                if (vm.IsMobileBankingClient != null)
                {
                    if(vm.IsMobileBankingClient.ToLower() == "yes")
                    {
                       clients = clients.Where(e => e.IsMobileBankingClient == true);
                    }
                    else
                    {
                       clients = clients.Where(e => e.IsMobileBankingClient == false);
                    }
                }

                List<AppClientReport> clientList = MapAppClientToAppClientReport(clients.ToList());

                vm.AppClientReport = clientList;

                return View(vm);
            }

            return View(vm);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppClientVm vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                AppClientDto dto = new()
                {
                    ClientName = vm.ClientName,
                    CurrentVersion = vm.CurrentVersion,
                    IsMobileBankingClient = vm.IsMobileBankingClient,
                    Link = vm.Link,
                    Location = vm.Location
                };

                var client = await _clientService.Create(dto);
                TempData["success"] = $"Client Added : {client.ClientName}";

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Waaaiyat...........😡😡");
                }

                var client = await _clientRepo.GetItemAsync(e => e.Id == id);
                if (client == null)
                {
                    throw new Exception("Client not found !");
                }

                AppClientEditVm vm = new()
                {
                    Id = client.Id,
                    ClientName = client.ClientName,
                    CurrentVersion = client.CurrentVersion,
                    IsMobileBankingClient= client.IsMobileBankingClient,
                    Link = client.Link,
                    Location = client.Location
                };

                return View(vm);

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Client");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppClientEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var client = await _clientRepo.GetItemAsync(e => e.Id == vm.Id);
            if (client == null)
            {
                TempData["error"] = "Client not found to update !";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var dto = MapAppClientEditVmToAppClientDto(vm);
                await _clientService.Update(client, dto);

                TempData["success"] = "Client Updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vm);
            }
        }

        private AppClientDto MapAppClientEditVmToAppClientDto(AppClientEditVm vm)
        {
            var dto = new AppClientDto
            {
                ClientName = vm.ClientName,
                Location = vm.Location,
                CurrentVersion = vm.CurrentVersion,
                IsMobileBankingClient = vm.IsMobileBankingClient,
                Link = vm.Link
            };
            return dto;
        }


        #region PrivateMethods
        private static List<AppClientReport> MapAppClientToAppClientReport(List<AppClient> clients)
        {
            var clientList = new List<AppClientReport>();
            foreach (var client in clients)
            {
                var c = new AppClientReport
                {
                    ClientId = client.Id,
                    ClientName = client.ClientName,
                    CurrentVersion = client.CurrentVersion,
                    Link = client.Link,
                    Location = client.Location,
                    IsMobileBankingClient = client.IsMobileBankingClient
                };
                clientList.Add(c);
            }

            return clientList;
        }
        #endregion
    }
}
