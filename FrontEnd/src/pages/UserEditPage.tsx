// src/pages/UserEditPage.tsx
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';

export const UserEditPage = () => {
  const navigate = useNavigate();
  const user = authService.getCurrentUser();
  
  const [formData, setFormData] = useState({
    username: '',
    email: ''
  });

  const [passData, setPassData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  const [loading, setLoading] = useState(true);
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingPass, setSavingPass] = useState(false);

  // 1. Tải thông tin hiện tại
  useEffect(() => {
    if (!user?.userId) {
      navigate('/login');
      return;
    }

    const loadData = async () => {
      try {
        const profile = await userService.getProfile(user.userId);
        setFormData({
            username: profile.username || '',
            email: profile.email || ''
        });
      } catch (error) {
        console.error("Lỗi tải thông tin:", error);
      } finally {
        setLoading(false);
      }
    };
    loadData();
    
  }, [user?.userId, navigate]); 

  // --- XỬ LÝ: Lưu thông tin chung ---
  const handleProfileSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user?.userId) return;

    setSavingProfile(true);
    try {
        await userService.updateProfile(user.userId, formData);
        
        alert("Cập nhật thành công! Vui lòng đăng nhập lại để áp dụng thay đổi.");
        authService.logout(); 
        navigate('/login');   
        
    } catch (error: any) {
        console.error(error);
        const msg = error.response?.data?.message || "Lỗi khi cập nhật. Có thể Email/Username đã tồn tại.";
        alert(msg);
    } finally {
        setSavingProfile(false);
    }
  };

  // --- XỬ LÝ: Đổi mật khẩu ---
  const handlePasswordSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user?.userId) return;

    if (passData.newPassword !== passData.confirmPassword) {
        alert("Mật khẩu mới và xác nhận mật khẩu không khớp!");
        return;
    }

    if (passData.newPassword.length < 6) {
        alert("Mật khẩu mới phải có ít nhất 6 ký tự.");
        return;
    }

    setSavingPass(true);
    try {
        await userService.changePassword(user.userId, {
            currentPassword: passData.currentPassword,
            newPassword: passData.newPassword
        });
        
        alert("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");
        authService.logout();
        navigate('/login');
        
    } catch (error: any) {
        console.error(error);
        const msg = error.response?.data?.message || "Đổi mật khẩu thất bại. Kiểm tra lại mật khẩu cũ.";
        alert(msg);
    } finally {
        setSavingPass(false);
    }
  };

  if (loading) return <div style={{padding:'50px', textAlign:'center'}}>Đang tải...</div>;

  return (
    <div style={{ padding: '40px', background: '#F4F7FF', minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '30px' }}>
      
      {/* FORM 1: THÔNG TIN CÁ NHÂN */}
      <div style={cardStyle}>
        <h2 style={{ margin: '0 0 20px 0', color: '#333', borderBottom: '1px solid #eee', paddingBottom: '10px' }}>
            Thông tin cá nhân
        </h2>
        
        <form onSubmit={handleProfileSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <div>
                <label style={labelStyle}>Username</label>
                <input 
                    type="text" 
                    value={formData.username}
                    onChange={e => setFormData({...formData, username: e.target.value})}
                    style={inputStyle}
                    required
                />
            </div>

            <div>
                <label style={labelStyle}>Email Address</label>
                <input 
                    type="email" 
                    value={formData.email}
                    onChange={e => setFormData({...formData, email: e.target.value})}
                    style={inputStyle}
                    required
                />
            </div>

            <button 
                type="submit" 
                disabled={savingProfile}
                style={{ ...btnStyle, background: savingProfile ? '#ccc' : '#3056D3', color: 'white' }}
            >
                {savingProfile ? 'Đang lưu...' : 'Lưu thay đổi'}
            </button>
        </form>
      </div>

      {/* FORM 2: ĐỔI MẬT KHẨU */}
      <div style={cardStyle}>
        <h2 style={{ margin: '0 0 20px 0', color: '#333', borderBottom: '1px solid #eee', paddingBottom: '10px' }}>
            Đổi mật khẩu
        </h2>
        
        <form onSubmit={handlePasswordSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <div>
                <label style={labelStyle}>Mật khẩu hiện tại</label>
                <input 
                    type="password" 
                    value={passData.currentPassword}
                    onChange={e => setPassData({...passData, currentPassword: e.target.value})}
                    style={inputStyle}
                    required
                    placeholder="Nhập mật khẩu cũ để xác nhận"
                />
            </div>

            <div style={{ display: 'flex', gap: '15px' }}>
                <div style={{ flex: 1 }}>
                    <label style={labelStyle}>Mật khẩu mới</label>
                    <input 
                        type="password" 
                        value={passData.newPassword}
                        onChange={e => setPassData({...passData, newPassword: e.target.value})}
                        style={inputStyle}
                        required
                    />
                </div>
                <div style={{ flex: 1 }}>
                    <label style={labelStyle}>Xác nhận mật khẩu mới</label>
                    <input 
                        type="password" 
                        value={passData.confirmPassword}
                        onChange={e => setPassData({...passData, confirmPassword: e.target.value})}
                        style={inputStyle}
                        required
                    />
                </div>
            </div>

            <div style={{ display: 'flex', gap: '10px', marginTop: '10px' }}>
                <button 
                    type="submit" 
                    disabled={savingPass}
                    style={{ ...btnStyle, background: savingPass ? '#ccc' : '#D9534F', color: 'white' }}
                >
                    {savingPass ? 'Đang xử lý...' : 'Đổi mật khẩu'}
                </button>
                <button 
                    type="button" 
                    onClick={() => navigate('/profile')}
                    style={{ ...btnStyle, background: 'white', border: '1px solid #ddd', color: '#333' }}
                >
                    Quay lại
                </button>
            </div>
        </form>
      </div>

    </div>
  );
};

// --- STYLES ---
const cardStyle = {
    background: 'white',
    padding: '30px',
    borderRadius: '8px',
    boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
    width: '100%',
    maxWidth: '600px'
};

const labelStyle = {
    display: 'block',
    marginBottom: '8px',
    fontWeight: '500',
    color: '#555',
    fontSize: '14px'
};

const inputStyle = {
    width: '100%',
    padding: '12px',
    borderRadius: '6px',
    border: '1px solid #e0e0e0',
    fontSize: '15px',
    boxSizing: 'border-box' as const
};

const btnStyle = {
    flex: 1,
    padding: '12px',
    borderRadius: '6px',
    border: 'none',
    fontSize: '15px',
    fontWeight: 'bold',
    cursor: 'pointer',
    transition: 'all 0.2s'
};