import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '../services/authService';

export const RegisterPage = () => {
  const [formData, setFormData] = useState({ username: '', email: '', password: '', confirmPass: '' });
  const navigate = useNavigate();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.password !== formData.confirmPass) {
      alert("Mật khẩu xác nhận không khớp!");
      return;
    }

    try {
      await authService.register(formData.username, formData.email, formData.password);
      alert("Đăng ký thành công! Hãy đăng nhập ngay.");
      navigate('/login');
    } catch (error: any) {
      alert("Lỗi đăng ký: " + (error.response?.data || error.message));
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={{textAlign: 'center', color: '#333'}}>Đăng Ký</h2>
        <form onSubmit={handleRegister} style={{display: 'flex', flexDirection: 'column', gap: '15px'}}>
          <input 
            type="text" placeholder="Tên người dùng" required style={styles.input}
            onChange={e => setFormData({...formData, username: e.target.value})}
          />
          <input 
            type="email" placeholder="Email" required style={styles.input}
            onChange={e => setFormData({...formData, email: e.target.value})}
          />
          <input 
            type="password" placeholder="Mật khẩu" required style={styles.input}
            onChange={e => setFormData({...formData, password: e.target.value})}
          />
          <input 
            type="password" placeholder="Nhập lại mật khẩu" required style={styles.input}
            onChange={e => setFormData({...formData, confirmPass: e.target.value})}
          />
          <button type="submit" style={styles.button}>Đăng ký</button>
        </form>
        <p style={{textAlign: 'center', marginTop: '15px'}}>
          Đã có tài khoản? <Link to="/login">Đăng nhập</Link>
        </p>
      </div>
    </div>
  );
};

// Dùng chung style với Login cho nhanh
const styles = {
  container: { display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: '#f0f2f5' },
  card: { background: 'white', padding: '40px', borderRadius: '10px', boxShadow: '0 4px 10px rgba(0,0,0,0.1)', width: '350px' },
  input: { padding: '10px', borderRadius: '5px', border: '1px solid #ddd', fontSize: '16px' },
  button: { padding: '10px', background: '#42b72a', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer', fontSize: '16px', fontWeight: 'bold' as 'bold' }
};