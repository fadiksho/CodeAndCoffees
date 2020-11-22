export enum ToastType {
  info,
  success,
  warning,
  error
}

interface ToastConfig {
  onAccept?(): void;
  onReject?(): void;
  sticky?: boolean;
  type?: ToastType;
  timout?: number;
}
class Toast {
  toastContainer: HTMLElement;
  toast(message: string, config: ToastConfig = {}): void {
    this.initContainer();
    const defaultConfig: ToastConfig = {
      onAccept: null,
      onReject: null,
      sticky: false,
      timout: 5000,
      type: ToastType.info
    };

    // eslint-disable-next-line @typescript-eslint/no-use-before-define
    new ToastItem(this.toastContainer, message, {
      ...defaultConfig,
      ...config
    });
  }

  initContainer(): void {
    if (!this.toastContainer) {
      this.toastContainer = document.createElement("div");
      this.toastContainer.setAttribute("id", "snackbar");
      document.body.appendChild(this.toastContainer);
    }
  }
}

class ToastItem {
  toastContainer: HTMLElement;
  toastElement: HTMLElement;
  config: ToastConfig;
  isRemoved = false;
  timerId: NodeJS.Timeout;

  constructor(
    container: HTMLElement,
    message: string,
    toastOptions: ToastConfig
  ) {
    this.toastContainer = container;
    this.config = toastOptions;
    this.toastElement = this.buildToast(message);
    // this magic number 800 is the animation duration
    this.config.timout += 800;
    this.showToast();
  }

  buildToast(message: string): HTMLElement {
    const newToast = document.createElement("div");
    const toastMessage = document.createElement("p");
    toastMessage.className = "toast-message";
    toastMessage.innerText = message;
    newToast.className = `toast ${this.getToastType(this.config.type)}`;
    const newToastAction = document.createElement("div");
    newToastAction.className = "toast-actions";
    const acceptAction = document.createElement("button");
    acceptAction.innerText = "OK";
    acceptAction.className = "toast-action-button toast-accept btn";
    // Only show reject button when reject callback is present
    if (this.config.onAccept) {
      const cancelAction = document.createElement("button");
      cancelAction.innerText = "Cancel";
      cancelAction.className = "toast-action-button toast-reject btn";
      cancelAction.addEventListener("click", () => {
        this.closeToast(this.config.onReject);
        if (this.timerId) {
          clearTimeout(this.config.timout);
        }
      });
      newToastAction.appendChild(cancelAction);
    }
    // Show X (close) icon when reject callback is not present.
    else {
      const closeButton = document.createElement("button");
      closeButton.className = "toast-close";
      closeButton.innerText = "X";
      closeButton.addEventListener("click", () => {
        this.closeToast();
        if (this.timerId) {
          clearTimeout(this.timerId);
        }
      });
      newToast.appendChild(closeButton);
    }

    acceptAction.addEventListener("click", () => {
      this.closeToast(this.config.onAccept);
      if (this.timerId) {
        clearTimeout(this.timerId);
      }
    });

    newToastAction.appendChild(acceptAction);
    newToast.appendChild(toastMessage);
    newToast.appendChild(newToastAction);

    return newToast;
  }

  showToast(): void {
    this.toastElement.classList.add("bounceInRight");
    this.toastContainer.appendChild(this.toastElement);

    if (!this.config.sticky) {
      this.timerId = setTimeout(() => {
        this.closeToast();
      }, this.config.timout);
    }
  }

  closeToast(onCallBack: () => void = null): void {
    // to prevent multiple click
    if (this.isRemoved) return;
    else this.isRemoved = true;

    if (onCallBack) {
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

  getToastType(type: ToastType): string {
    switch (type) {
      case ToastType.info:
        return "toast-info";
      case ToastType.success:
        return "toast-success";
      case ToastType.warning:
        return "toast-warning";
      case ToastType.error:
        return "toast-error";
      default:
        return "toast-info";
    }
  }
}

export default new Toast();
