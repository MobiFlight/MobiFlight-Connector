import i18n from 'i18next';
import Backend from 'i18next-http-backend';
import { initReactI18next } from 'react-i18next';
import { HMRPlugin } from 'i18next-hmr/plugin';

const instance = i18n
  .use(Backend)
  .use(initReactI18next)

if (process.env.NODE_ENV === 'development') {
  instance.use(new HMRPlugin({ vite: { client: true } }));
}

instance.init({
    fallbackLng: 'en',
    debug: false,
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n
