import { createStore } from 'vuex'

const store = createStore({
  state: {
    user: null, // Thông tin người dùng sau khi đăng nhập
    token: null, // Token JWT
    role: null,   // Role của người dùng (Admin, User, v.v.)
  },
  mutations: {
    setUser(state, user) {
      state.user = user
    },
    setToken(state, token) {
      state.token = token
    },
    setRole(state, role) {
      state.role = role
    },
  },
  actions: {
    login({ commit }, { user, token, role }) {
      commit('setUser', user)
      commit('setToken', token)
      commit('setRole', role)
    },
    logout({ commit }) {
      commit('setUser', null)
      commit('setToken', null)
      commit('setRole', null)
    }
  }
})

export default store
