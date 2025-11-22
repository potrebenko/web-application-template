import { createApp } from "vue";
import App from "./App.vue";
import vuetify from "@/plugins/vuetify";
import router from "@/router";
import "roboto-fontface/css/roboto/roboto-fontface.css";
import "@mdi/font/css/materialdesignicons.css";

const app = createApp(App);
app.use(vuetify);
app.use(router);
app.mount("#app");
