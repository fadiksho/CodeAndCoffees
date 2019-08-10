// assets
import "./Assets/images/auther-image.png";
import "./Assets/images/codeandcoffees-brand-1200x1200.png";
import "./Assets/images/codeandcoffees-brand-2400x1260.png";

import "./Assets/favicon.ico";
import "./Assets/images/icon.png";

import "./Pages/site.scss";
import "./Pages/BlogDetail/blog-detail.scss";
import "./Theme/theme-dark.scss";

import "./Pages/site.js";

// Add a namespace
window.MyBlog = {};

var Routes = {
  Blog: {
    init: function() {
      // controller-wide code
    },
    Detail: function() {
      // action-specific code
      import("./Pages/BlogDetail/blog-detail").then(module => {
        window.MyBlog.Detail = module.blogDetail;
      });
    }
  }
};

var Router = {
  exec: function(controller, action) {
    action = action === undefined ? "init" : action;

    if (
      controller !== "" &&
      Routes[controller] &&
      typeof Routes[controller][action] === "function"
    ) {
      Routes[controller][action]();
    }
  },

  init: function() {
    let body = document.body;
    let controller = body.getAttribute("data-controller");
    let action = body.getAttribute("data-action");

    Router.exec(controller);
    Router.exec(controller, action);
  }
};

//run this immediately
Router.init();
