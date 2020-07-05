using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpreadSheetLibrary.Annotations;

namespace SpreadSheetLibrary.Data.Entities
{
    public class ClientData : BaseModel
    {
        private string _DOB;

        public string DOB
        {
            get => _DOB;
            set => SetProperty(ref _DOB, value);
        }

        private string _Age;

        public string Age
        {
            get => _Age;
            set => SetProperty(ref _Age, value);
        }


        private string _Gender;

        public string Gender
        {
            get => _Gender;
            set => SetProperty(ref _Gender, value);
        }


        private string _NHINumber;

        public string NHINumber
        {
            get => _NHINumber;
            set => SetProperty(ref _NHINumber, value);
        }


        private string _ClientName;

        public string ClientName
        {
            get => _ClientName;
            set => SetProperty(ref _ClientName, value);
        }

        public ClientData()
        {
            if(ServiceData == null)
            ServiceData = new ServiceReasonData();

            if(ReferralData == null)
            ReferralData = new ReferralReason();

            if(GeneralStatus == null)
            GeneralStatus = new GeneralStatusData();
        }

        private ServiceReasonData _ServiceData;

        public ServiceReasonData ServiceData
        {
            get => _ServiceData;
            set => SetProperty(ref _ServiceData, value);
        }

        private ObservableCollection<WorkerInfo> _Workers = new ObservableCollection<WorkerInfo>();

        public ObservableCollection<WorkerInfo> Workers
        {
            get => _Workers;
            set => SetProperty(ref _Workers, value);
        }

        private ReferralReason _ReferralData;

        public ReferralReason ReferralData
        {
            get => _ReferralData;
            set => SetProperty(ref _ReferralData, value);
        }


        private GeneralStatusData _GeneralStatus;

        public GeneralStatusData GeneralStatus
        {
            get => _GeneralStatus;
            set => SetProperty(ref _GeneralStatus, value);
        }




    }

    public class OrganisationServiceData : BaseModel
    {
        private string _ServiceName;

        public string ServiceName
        {
            get => _ServiceName;
            set => SetProperty(ref _ServiceName, value);
        }

        private ObservableCollection<ClientData> _Clients = new ObservableCollection<ClientData>();

        public ObservableCollection<ClientData> Clients
        {
            get => _Clients;
            set => SetProperty(ref _Clients, value);
        }

        public OrganisationServiceData()
        {
            
        }

    }


    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

    }

    public class MainViewModel : BaseModel
    {
        private ObservableCollection<OrganisationServiceData> _Services = new ObservableCollection<OrganisationServiceData>();

        public ObservableCollection<OrganisationServiceData> Services
        {
            get => _Services;
            set => SetProperty(ref _Services, value);
        }

        private ClientData _CurrentClient;

        public ClientData CurrentClient
        {
            get => _CurrentClient;
            set => SetProperty(ref _CurrentClient, value);
        }

        private OrganisationServiceData _CurrentService;

        public OrganisationServiceData CurrentService
        {
            get => _CurrentService;
            set => SetProperty(ref _CurrentService, value);
        }



        public MainViewModel()
        {
            if (CurrentClient == null)
            {
                CurrentClient = new ClientData();
            }

            if (CurrentService == null)
            {
                CurrentService = new OrganisationServiceData();
            }
        }



    }

    public class ServiceReasonData : BaseModel
    {
        private string _ContractName;

        public string ContractName
        {
            get => _ContractName;
            set => SetProperty(ref _ContractName, value);
        }


        private string _ContractNumber;

        public string ContractNumber
        {
            get => _ContractNumber;
            set => SetProperty(ref _ContractNumber, value);
        }


        private string _ServiceEndDate;

        public string ServiceEndDate
        {
            get => _ServiceEndDate;
            set => SetProperty(ref _ServiceEndDate, value);
        }


        private string _ServiceStartDate;

        public string ServiceStartDate
        {
            get => _ServiceStartDate;
            set => SetProperty(ref _ServiceStartDate, value);
        }


        private string _EngagementStatus;

        public string EngagementStatus
        {
            get => _EngagementStatus;
            set => SetProperty(ref _EngagementStatus, value);
        }


        private string _ServiceReason;

        public string ServiceReason
        {
            get => _ServiceReason;
            set => SetProperty(ref _ServiceReason, value);
        }


    }

    public class WorkerInfo:BaseModel
    {


        private string _End;

        public string End
        {
            get => _End;
            set => SetProperty(ref _End, value);
        }


        private string _Start;

        public string Start
        {
            get => _Start;
            set => SetProperty(ref _Start, value);
        }



        private string _WorkerName;

        public string WorkerName
        {
            get => _WorkerName;
            set => SetProperty(ref _WorkerName, value);
        }


    }

    public class ReferralReason : BaseModel
    {

        private string _ReferrerIndividual;

        public string ReferrerIndividual
        {
            get => _ReferrerIndividual;
            set => SetProperty(ref _ReferrerIndividual, value);
        }




        private string _ReferrerOrganisation;

        public string ReferrerOrganisation
        {
            get => _ReferrerOrganisation;
            set => SetProperty(ref _ReferrerOrganisation, value);
        }





        private string _ReferralAcceptedDeclinedReason;

        public string ReferralAcceptedDeclinedReason
        {
            get => _ReferralAcceptedDeclinedReason;
            set => SetProperty(ref _ReferralAcceptedDeclinedReason, value);
        }


        private string _ReferralAcceptedDeclined;

        public string ReferralAcceptedDeclined
        {
            get => _ReferralAcceptedDeclined;
            set => SetProperty(ref _ReferralAcceptedDeclined, value);
        }


        private string _ReferralEndDate;

        public string ReferralEndDate
        {
            get => _ReferralEndDate;
            set => SetProperty(ref _ReferralEndDate, value);
        }


        private string _ReferralUpdated;

        public string ReferralUpdated
        {
            get => _ReferralUpdated;
            set => SetProperty(ref _ReferralUpdated, value);
        }


        private string _ReferralCreationDate;

        public string ReferralCreationDate
        {
            get => _ReferralCreationDate;
            set => SetProperty(ref _ReferralCreationDate, value);
        }


        private string _ReferralDate;

        public string ReferralDate
        {
            get => _ReferralDate;
            set => SetProperty(ref _ReferralDate, value);
        }






        private string _Reason;

        public string Reason
        {
            get => _Reason;
            set => SetProperty(ref _Reason, value);
        }


    }


    public class GeneralStatusData : BaseModel
    {

        private string _WhenEnded;

        public string WhenEnded
        {
            get => _WhenEnded;
            set => SetProperty(ref _WhenEnded, value);
        }




        private string _WhenStarted;

        public string WhenStarted
        {
            get => _WhenStarted;
            set => SetProperty(ref _WhenStarted, value);
        }



        private string _StatusName;

        public string StatusName
        {
            get => _StatusName;
            set => SetProperty(ref _StatusName, value);
        }


    }



}
