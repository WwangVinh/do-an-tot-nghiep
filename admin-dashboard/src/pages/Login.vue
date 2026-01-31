<template>
  <div class="login-container">
    <div class="login-form">
      <h2>Đăng Nhập</h2>
      <form @submit.prevent="login">
        <div class="input-group">
          <input v-model="username" type="text" placeholder="Tên người dùng" required />
        </div>
        <div class="input-group">
          <input v-model="password" type="password" placeholder="Mật khẩu" required />
        </div>
        <button type="submit" class="login-btn">Đăng nhập</button>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      username: '',
      password: ''
    };
  },
  methods: {
    login() {
      // Giả lập đăng nhập thành công
      const mockToken = 'fake-jwt-token'; // Giả lập token JWT
      const mockRole = 'Admin'; // Giả lập quyền người dùng
      this.$store.dispatch('login', {
        user: this.username,
        token: mockToken,
        role: mockRole
      });
      this.$router.push('/'); // Chuyển đến Dashboard
    }
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: #f4f4f9;
}

.login-form {
  background: white;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  width: 300px;
  animation: fadeIn 1s ease-in-out;
}

h2 {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.input-group {
  margin-bottom: 15px;
}

input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  transition: border-color 0.3s;
}

input:focus {
  border-color: #007BFF;
}

button {
  width: 100%;
  padding: 12px;
  background-color: #007BFF;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #0056b3;
}

@keyframes fadeIn {
  0% {
    opacity: 0;
    transform: translateY(-30px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
