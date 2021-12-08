using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("epic_detail")]
    public partial class EpicDetail
    {

        [Column("epic")]
        public string Epic { get; set; }

        [Column("expiry")]
        public string Expiry { get; set; } = "-";

        [Column("name")]
        public string Name { get; set; } = Constants.PropertyNullError;

        [Column("force_open_allowed")]
        public bool ForceOpenAllowed { get; set; }

        [Column("stop_limit_allowed")]
        public bool StopsLimitsAllowed { get; set; }

        [Column("lot_size")]
        public decimal? LotSize { get; set; }

        [Column("position_size_unit")]
        public string PositionSizeUnit { get; set; } = Constants.PropertyNullError;

        [Column("type")]
        public string Type { get; set; } = Constants.PropertyNullError;

        [Column("controlled_risk_allowed")]
        public bool ControlledRiskAllowed { get; set; }

        [Column("streaming_prices_available")]
        public bool StreamingPricesAvailable { get; set; }

        [Column("market_id")]
        public string MarketId { get; set; } = Constants.PropertyNullError;

        [Column("sprintmarket_minimum_expiry_time")]
        public int SprintMarketsMinimumExpiryTime { get; set; }

        [Column("sprintmarket_maximum_expiry_time")]
        public int SprintMarketsMaximumExpiryTime { get; set; }		

        [Column("margin_factor")]
        public decimal? MarginFactor { get; set; }

        [Column("margin_factor_unit")]
        public string MarginFactorUnit { get; set; } = Constants.PropertyNullError;

        [Column("slippage_factor_unit")]
        public string? SlippageFactorUnit { get; set; }

        [Column("slippage_factor_value")]
        public decimal? SlippageFactorValue { get; set; }					

        [Column("expiry_last_dealingdate")]
        public DateTime? ExpirylastDealingDate { get; set; }

        [Column("expiry_settlement_info")]
        public string? ExpirysettlementInfo { get; set; }

        [Column("last_rollover_time")]
        public DateTime? LastRolloverTime { get; set; }
        
        [Column("rollover_info")]
        public string? RolloverInfo { get; set; }

        [Column("news_code")]
        public string? NewsCode { get; set; } = Constants.PropertyNullError;

        [Column("chart_code")]
        public string? ChartCode { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        [Column("value_of_one_pip")]
        public string? ValueOfOnePip { get; set; } = Constants.PropertyNullError;

        [Column("one_pip_means")]
        public string? OnePipMeans { get; set; } = Constants.PropertyNullError;

        [Column("contract_size")]
        public decimal? ContractSize { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

        #region Childs
        public virtual List<EpicDetailMarginDepositBand>? MarginDepositBands { get; set; }

        public virtual List<EpicDetailOpeningHour>? OpeningHours { get; set; }

        public virtual List<EpicDetailSpecialInfo>? SpecialInfo { get; set; }

        public virtual List<EpicDetailCurrency>? Currencies { get; set; }
        #endregion
    }
}

