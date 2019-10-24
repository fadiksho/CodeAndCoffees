class Toast {
  toast({
    message,
    onAccept,
    onReject,
    sticky = false,
    type = "info",
    timeout = 5000
  }) {
    this.initContainer();
    new ToastItem(this.toastContainer, {
      message,
      onAccept,
      onReject,
      sticky,
      type,
      timeout
    });
  }

  initContainer() {
    if (!this.toastContainer) {
      this.toastContainer = document.createElement("div");
      this.toastContainer.setAttribute("id", "snackbar");
      document.body.appendChild(this.toastContainer);
    }
  }
}

class ToastItem {
  constructor(container, toastOption) {
    this.timerId = 0;
    this.toastContainer = container;
    this.onAccept = toastOption.onAccept;
    this.onReject = toastOption.onReject;
    this.toastElement = this.buildToast(toastOption);
    this.sticky = toastOption.sticky;
    this.isRemoved = false;
    // this magic number 800 is the animation duration
    this.timeout = toastOption.timeout + 800;
    this.showToast();
  }

  buildToast(toastOption) {
    let newToast = document.createElement("div");

    let toastMessage = document.createElement("p");
    toastMessage.className = "toast-message";
    toastMessage.innerText = toastOption.message;

    newToast.className = `toast ${this.getToastType(toastOption.type)}`;

    let newToastAction = document.createElement("div");
    newToastAction.className = "toast-actions";
    let acceptAction = document.createElement("button");
    acceptAction.innerText = "OK";
    acceptAction.className = "toast-action-button toast-accept btn";
    // Only show reject button when reject callback is present
    if (this.onReject && typeof this.onReject === "function") {
      let cancelAction = document.createElement("button");
      cancelAction.innerText = "Cancel";
      cancelAction.className = "toast-action-button toast-reject btn";
      cancelAction.addEventListener("click", e => {
        this.closeToast(this.onReject);
        if (this.timerId) {
          clearTimeout(this.timerId);
        }
      });
      newToastAction.appendChild(cancelAction);
    }
    // Show X (close) icon when reject callback is not present.
    else {
      let closeButton = document.createElement("button");
      closeButton.className = "toast-close";
      closeButton.innerText = "X";
      closeButton.addEventListener("click", e => {
        this.closeToast();
        if (this.timerId) {
          clearTimeout(this.timerId);
        }
      });
      newToast.appendChild(closeButton);
    }

    acceptAction.addEventListener("click", e => {
      this.closeToast(this.onAccept);
      if (this.timerId) {
        clearTimeout(this.timerId);
      }
    });

    newToastAction.appendChild(acceptAction);
    newToast.appendChild(toastMessage);
    newToast.appendChild(newToastAction);

    return newToast;
  }

  showToast() {
    this.toastElement.classList.add("bounceInRight");
    this.toastContainer.appendChild(this.toastElement);

    if (!this.sticky) {
      this.timerId = setTimeout(() => {
        this.closeToast();
      }, this.timeout);
    }
  }

  closeToast(onCallBack) {
    // to prevent multiple click
    if (this.isRemoved) return;
    else this.isRemoved = true;

    if (onCallBack && typeof onCallBack === "function") {
      onCallBack();
    }
    this.toastElement.classList.add("bounceOutRight");
    //
    this.toastElement.addEventListener(
      "animationend",
      () => {
        this.toastContainer.removeChild(this.toastElement);
      },
      false
    );
  }

  getToastType(type) {
    switch (type) {
      case "info":
        return "toast-info";
      case "success":
        return "toast-success";
      case "warning":
        return "toast-warning";
      case "error":
        return "toast-error";
      default:
        return "toast-info";
    }
  }
}

export default new Toast();
