import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../components/main.vue')
    },
    {
      path: '/signin',
      component: () => import('../components/signin.vue')
    },
    {
      path: '/signup',
      component: () => import('../components/signup.vue')
    },
    {
      path: '/recover',
      name: 'Recover',
      component: () => import('../components/recover.vue')
    },
    {
      path: '/forum',
      component: () => import('../components/forum.vue')
    },
    {
      path: '/Detail',
      component: () => import('../components/detail.vue')
    },
    {
      path: '/EditPost',
      component: () => import('../components/EditPost.vue')
    },
    {
      path: '/personal',
      component: () => import('../components/personal.vue'),
    },
    {
      path: '/personalEdit',
      component: () => import('../components/personalEdit.vue')
    },
    {
      path: '/Games',
      component: () => import('../components/Games.vue')
    },
    {
      path: '/News',
      component: () => import('../components/News.vue')
    },
    {
      path: '/Players',
      component: () => import('../components/Players.vue')
    },
    {
      path: '/AdminPosts',
      component: () => import('../components/AdminComponents/AdminPosts.vue')
    },
    {
      path: '/AdminUsers',
      component: () => import('../components/AdminComponents/AdminUsers.vue')
    },
    {
      path: '/AdminNews',
      component: () => import('../components/AdminComponents/AdminNews.vue')
    },
    {
      path: '/AdminAnnouncement',
      component: () => import('../components/AdminComponents/AdminAnnouncement.vue')
    },
    {
      path: '/AdminPastAnc',
      component: () => import('../components/AdminComponents/AdminPastAnc.vue')
    },
    {
      path: '/teamMsg',
      component: () => import('../components/teamMsg.vue')
    },
    {
      path: '/detailedPlayerMsg',
      component: () => import('../components/detailedPlayerMsg.vue')
    },
    {
      path: '/detailedMatch',
      component: () => import('../components/detailedMatch.vue')
    },
    {
      path: '/Games',
      component: () => import('../components/Games.vue')
    },
    {
      path: '/NewsVideo',
      component: () => import('../components/NewsVideo.vue')
    },
    {
      path: '/NewsGossip',
      component: () => import('../components/NewsGossip.vue')
    },
    {
      path: '/NewsDetails',
      component: () => import('../components/NewsDetails.vue')
    },
  ]
})
export default router;
