import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'

const resources = {
  fa: {
    translation: {
      "admin": "ادمین",
      "dashboard": "داشبورد",
      "users": "کاربران",
      "verifications": "تایید مدارک",
      "reports": "گزارش‌ها",
      "content": "محتوا",
      "settings": "تنظیمات"
    }
  }
}

i18n.use(initReactI18next).init({
  resources,
  lng: 'fa',
  fallbackLng: 'fa',
  interpolation: { escapeValue: false }
})
