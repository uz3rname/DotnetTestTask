using Blazored.Modal;
using Blazored.Modal.Services;
using DotnetTestTask.Components.Modals;

namespace DotnetTestTask.Utils
{
    public interface IWebUtils
    {
        public Task ShowErrorModal(string Message);
    }
    public class WebUtils : IWebUtils
    {
        IModalService _modalService;

        public WebUtils(IModalService modalService)
        {
            _modalService = modalService;
        }

        public async Task ShowErrorModal(string Message)
        {
            var parameters = new ModalParameters()
                .Add(nameof(ErrorModal.Message), Message);
            var modal = _modalService.Show<ErrorModal>("Error", parameters);
            await modal.Result;
        }
    }
}